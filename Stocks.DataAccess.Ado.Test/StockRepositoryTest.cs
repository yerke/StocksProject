using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stocks.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Stocks.DataAccess.Ado.Test
{
    [TestClass]
    public class StockRepositoryTest
    {
        #region Fields

        ClientRepository _clientRepo;
        StockRepository _stockRepo;

        List<Client> _clients;
        List<Stock> _stocks;

        #endregion

        #region Initialize and Cleanup

        [TestInitialize]
        public void Initialize()
        {
            _clientRepo = new ClientRepository();
            _stockRepo = new StockRepository();

            _clients = _clientRepo.Fetch().ToList();
            _stocks = _stockRepo.Fetch().ToList();
        }

        /// <summary>
        /// Deletes any left-over test records.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var _stockRepo = new StockRepository();
            var list = _stockRepo.Fetch(null).ToList();

            var toDelete = list.Where(o => o.Code == "TestCode" || o.Code == "XYZ");
            foreach (var item in toDelete)
            {
                item.IsMarkedForDeletion = true;
                _stockRepo.Persist(item);
            }
        }

        private Stock CreateSampleStock()
        {
            Stock stock = new Stock();
            stock.Code = "TestCode";
            stock.CompanyName = "RosNeftTest";
            stock.LastPrice = 101.22M;

            // Add three child Holding members
            var client1Id = _clients[0].ClientId;
            Holding hld1 = new Holding
            {
                ClientId = client1Id,
                Quantity = 34759,
                LastChangeDate = new DateTime(2014, 01, 15)
            };
            stock.Holdings.Add(hld1);

            var client2Id = _clients[1].ClientId;
            Holding hld2 = new Holding
            {
                ClientId = client2Id,
                Quantity = 59600,
                LastChangeDate = new DateTime(2014, 06, 08)
            };
            stock.Holdings.Add(hld2);

            var client3Id = _clients[2].ClientId;
            Holding hld3 = new Holding
            {
                ClientId = client3Id,
                Quantity = 77700,
                LastChangeDate = new DateTime(2014, 09, 29)
            };
            stock.Holdings.Add(hld3);

            return stock;
        }

        #endregion

        #region Fetch Tests

        [TestMethod]
        public void StockRepository_FetchNull_ReturnsAll()
        {
            // Arrange

            // Act
            var list = _stockRepo.Fetch();

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void StockRepository_FetchOne_ReturnsOne()
        {
            // Arrange
            var all = _stockRepo.Fetch().ToList();
            var StockId = all[0].StockId;
            var code = all[0].Code;

            var item = _stockRepo.Fetch(StockId).Single();

            Assert.IsNotNull(item);
            Assert.IsTrue(item.StockId == StockId);
            Assert.IsTrue(item.Code == code);
            Assert.IsFalse(item.IsMarkedForDeletion);
            Assert.IsFalse(item.IsDirty);
        }

        [TestMethod]
        public void StockRepository_FetchNonExistent_ReturnsEmptyList()
        {
            // Arrange

            // Try to fetch non-existent Show
            var resultList = _stockRepo.Fetch(-99);

            Assert.IsNotNull(resultList);
        }

        #endregion

        #region Persist Tests

        [TestMethod]
        public void StockRepository_InsertDelete()
        {
            // Arrange
            Stock newStock = CreateSampleStock();

            // Act for Insert
            var existingStock = _stockRepo.Persist(newStock);
            var testStockId = existingStock.StockId;
            // re-fetching creates a different C# Stock instance
            var refetch = _stockRepo.Fetch(testStockId).First();

            // Assert for Insert - Make sure local object is updated
            Assert.IsTrue(existingStock.StockId > 0);
            Assert.IsFalse(existingStock.IsMarkedForDeletion);
            Assert.IsFalse(existingStock.IsDirty);
            Assert.IsTrue(existingStock.Holdings[0].HoldingId > 0);
            Assert.IsTrue(existingStock.Holdings[1].HoldingId > 0);
            Assert.IsTrue(existingStock.Holdings[2].HoldingId > 0);

            // Assert for Insert - Make sure refetched object is correct
            Assert.IsTrue(refetch.StockId == testStockId);
            Assert.IsFalse(refetch.IsMarkedForDeletion);
            Assert.IsFalse(refetch.IsDirty);
            Assert.IsTrue(refetch.Code == "TestCode");
            Assert.IsTrue(refetch.CompanyName == "RosNeftTest");
            Assert.IsTrue(refetch.LastPrice == 101.22M);
            Assert.IsTrue(refetch.Holdings.Count() == 3);
            Assert.IsTrue(refetch.Holdings[0].HoldingId > 0);
            Assert.IsTrue(refetch.Holdings[1].HoldingId > 0);
            Assert.IsTrue(refetch.Holdings[1].HoldingId > 0);

            // Clean-up (Act for Delete)
            existingStock.IsMarkedForDeletion = true;
            _stockRepo.Persist(existingStock);

            // Assert for Delete
            var result = _stockRepo.Fetch(testStockId);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void StockRepository_InsertUpdateDelete()
        {
            // Arrange
            Stock newStock = CreateSampleStock();

            // Act - Insert Stock
            var existingStock = _stockRepo.Persist(newStock);
            var testStockId = existingStock.StockId;

            // Act for Update
            // Modify each scalar property
            existingStock.Code = "XYZ";
            existingStock.CompanyName = "ExxonTest";
            existingStock.LastPrice = 202.33M;
            existingStock.IsDirty = true;

            // Delete a Holding
            var holdingDeletedHoldingId = existingStock.Holdings[0].HoldingId;
            existingStock.Holdings[0].IsMarkedForDeletion = true;

            // Leave one Holding unchanged
            var holdingUnchangedHoldingId = existingStock.Holdings[1].HoldingId;

            // Modify one Holding
            var holdingModifiedHoldingId = existingStock.Holdings[2].HoldingId;
            existingStock.Holdings[2].Quantity = 888000;
            existingStock.Holdings[2].IsDirty = true;

            // Perform the update and refetch again
            // updatedItem and testShow refer to the same C# object
            existingStock = _stockRepo.Persist(existingStock);

            // re-fetching the same show from the database creates a
            // new C# object, that should be an exact replica of testStock
            var refetch = _stockRepo.Fetch(testStockId).FirstOrDefault();

            // Assert - Make sure updated item has proper flags set
            Assert.IsTrue(existingStock.IsDirty == false);
            Assert.IsTrue(existingStock.Code == "XYZ");

            // Assert - Make sure re-fetched Stock has expected properties
            Assert.IsNotNull(refetch);
            Assert.IsTrue(refetch.IsDirty == false);
            Assert.IsTrue(refetch.IsMarkedForDeletion == false);
            Assert.IsTrue(refetch.Code == "XYZ");
            Assert.IsTrue(refetch.CompanyName == "ExxonTest");
            Assert.IsTrue(refetch.LastPrice == 202.33M);
            Assert.IsTrue(refetch.Holdings.Count() == 2);
            Assert.IsTrue(refetch.Holdings
                .Where(o => o.HoldingId == holdingUnchangedHoldingId).Count() == 1);
            Assert.IsTrue(refetch.Holdings
                .Where(o => o.HoldingId == holdingModifiedHoldingId).Count() == 1);
            Assert.IsTrue(refetch.Holdings
                .Where(o => o.HoldingId == holdingDeletedHoldingId).Count() == 0);
            var refetchModifiedHolding = refetch.Holdings
                .Where(o => o.HoldingId == holdingModifiedHoldingId).FirstOrDefault();
            Assert.IsTrue(refetchModifiedHolding.Quantity == 888000);

            // Clean-up (Act for Delete)
            existingStock.IsMarkedForDeletion = true;
            _stockRepo.Persist(existingStock);

            // Assert for Delete
            var result = _stockRepo.Fetch(testStockId);
            Assert.IsFalse(result.Any());
        }

        /*
        [TestMethod]
        // I don't have a need for this test, since my Client
        // table does not depend on other tables. Yerke
        public void ClientRepository_InvalidRating_ThrowsSqlException()
        {
            // Arrange
            Client newClient = CreateSampleClient();

            try
            {
                // Act - Insert Show with an invalid rating
                // (Doesn't refer to an existing person id).
                newClient.MpaaRatingId = -1; // Client table in my case
                    // does not depend on other tables
                var existingShow = _showRepo.Persist(newClient);
                // Excution should not get pas the above line.
                // If it does, test fails.
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(SqlException));
            }
        }
        */

        #endregion

        #region Transaction Tests

            [TestMethod]
            public void StockRepository_InvalidClient_TransactionRollsBack()
            {
                // Arrange
                Stock newStock = CreateSampleStock();

                // Act - Insert Show with a bad Cast member record
                // (Doesn't refer to an existing person id).
                newStock.Holdings[0].ClientId = -1;
                try
                {
                    // Should throw exception
                    var existingStock = _stockRepo.Persist(newStock);
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex, typeof(ApplicationException));
                }

                // Make sure parent show object was NOT saved.
                var savedStock = _stockRepo.Fetch()
                    .Where(o => o.Code == "TestTitle")
                    .FirstOrDefault();
                Assert.IsNull(savedStock);
            }

        #endregion

            #region HasChanges Test

            [TestMethod]
            public void StockRepository_HoldingDirty_SetsGraphDirty()
            {
                // Arrange
                var repo = new StockRepository();
                var all = repo.Fetch(null).ToList();
                var StockId = all[0].StockId;
                var companyName = all[0].CompanyName;

                var item = repo.Fetch(StockId).Single();

                // Change one Holding to change a leaf
                // of the object graph
                item.Holdings[0].Quantity++;

                Assert.IsNotNull(item);
                Assert.IsTrue(item.StockId == StockId);
                Assert.IsTrue(item.CompanyName == companyName);
                Assert.IsFalse(item.IsMarkedForDeletion);

                // The IsDirty flag should be false
                Assert.IsFalse(item.IsDirty);

                // The HasChanges property should
                // be true, indicating the change to ShowGenres
                Assert.IsTrue(item.HasChanges);
            }

            #endregion

    }
}
