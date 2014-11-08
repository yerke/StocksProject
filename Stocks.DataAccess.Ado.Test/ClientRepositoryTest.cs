using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stocks.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Stocks.DataAccess.Ado.Test
{
    [TestClass]
    public class ClientRepositoryTest
    {
        #region Fields

        ClientRepository _clientRepo;
        StockRepository _stockRepo;

        List<Stock> _stocks;
        /*
        GenreRepository _genreRepo;
        CreditTypeRepository _creditTypeRepo;
        PersonRepository _personRepo;

        List<Genre> _genres;
        List<CreditType> _creditTypes;
        List<Person> _people;
        */
        #endregion

        #region Initialize and Cleanup

        [TestInitialize]
        public void Initialize()
        {
            _clientRepo = new ClientRepository();
            _stockRepo = new StockRepository();

            _stocks = _stockRepo.Fetch().ToList();
            /*
            _genreRepo = new GenreRepository();
            _creditTypeRepo = new CreditTypeRepository();
            _personRepo = new PersonRepository();

            _genres = _genreRepo.Fetch().ToList();
            _creditTypes = _creditTypeRepo.Fetch().ToList();
            _people = _personRepo.Fetch().ToList();
             */
        }

        /// <summary>
        /// Deletes any left-over test records.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            var _clientRepo = new ClientRepository();
            var list = _clientRepo.Fetch(null).ToList();

            var toDelete = list.Where(o => o.Code == "TestCode" || o.Code == "XYZ");
            foreach (var item in toDelete)
            {
                item.IsMarkedForDeletion = true;
                _clientRepo.Persist(item);
            }
        }

        private Client CreateSampleClient()
        {
            Client client = new Client();
            client.Code = "TestCode";
            client.FirstName = "Vlad";
            client.LastName = "Putin";
            client.Phone = "(495) 111 2222";
            client.Address = "Kremlin";

            // Add three child Holding members
            var stock1Id = _stocks[0].StockId;
            Holding hld1 = new Holding
            {
                StockId = stock1Id,
                Quantity = 34759,
                LastChangeDate = new DateTime(2014, 01, 15)
            };
            client.Holdings.Add(hld1);

            var stock2Id = _stocks[1].StockId;
            Holding hld2 = new Holding
            {
                StockId = stock2Id,
                Quantity = 59600,
                LastChangeDate = new DateTime(2014, 06, 08)
            };
            client.Holdings.Add(hld2);

            var stock3Id = _stocks[2].StockId;
            Holding hld3 = new Holding
            {
                StockId = stock3Id,
                Quantity = 77700,
                LastChangeDate = new DateTime(2014, 09, 29)
            };
            client.Holdings.Add(hld3);

            return client;
        }

        #endregion

        #region Fetch Tests

        [TestMethod]
        public void ClientRepository_FetchNull_ReturnsAll()
        {
            // Arrange

            // Act
            var list = _clientRepo.Fetch();

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void ClientRepository_FetchOne_ReturnsOne()
        {
            // Arrange
            var all = _clientRepo.Fetch().ToList();
            var ClientId = all[0].ClientId;
            var code = all[0].Code;

            var item = _clientRepo.Fetch(ClientId).Single();

            Assert.IsNotNull(item);
            Assert.IsTrue(item.ClientId == ClientId);
            Assert.IsTrue(item.Code == code);
            Assert.IsFalse(item.IsMarkedForDeletion);
            Assert.IsFalse(item.IsDirty);
        }

        [TestMethod]
        public void ClientRepository_FetchNonExistent_ReturnsEmptyList()
        {
            // Arrange

            // Try to fetch non-existent Show
            var resultList = _clientRepo.Fetch(-99);

            Assert.IsNotNull(resultList);
        }

        #endregion

        #region Persist Tests

        [TestMethod]
        public void ClientRepository_InsertDelete()
        {
            // Arrange
            Client newClient = CreateSampleClient();

            // Act for Insert
            var existingClient = _clientRepo.Persist(newClient);
            var testClientId = existingClient.ClientId;
            // re-fetching creates a different C# Client instance
            var refetch = _clientRepo.Fetch(testClientId).First();

            // Assert for Insert - Make sure local object is updated
            Assert.IsTrue(existingClient.ClientId > 0);
            Assert.IsFalse(existingClient.IsMarkedForDeletion);
            Assert.IsFalse(existingClient.IsDirty);
            Assert.IsTrue(existingClient.Holdings[0].HoldingId > 0);
            Assert.IsTrue(existingClient.Holdings[1].HoldingId > 0);
            Assert.IsTrue(existingClient.Holdings[2].HoldingId > 0);

            // Assert for Insert - Make sure refetched object is correct
            Assert.IsTrue(refetch.ClientId == testClientId);
            Assert.IsFalse(refetch.IsMarkedForDeletion);
            Assert.IsFalse(refetch.IsDirty);
            Assert.IsTrue(refetch.Code == "TestCode");
            Assert.IsTrue(refetch.FirstName == "Vlad");
            Assert.IsTrue(refetch.LastName == "Putin");
            Assert.IsTrue(refetch.Phone == "(495) 111 2222");
            Assert.IsTrue(refetch.Address == "Kremlin");
            Assert.IsTrue(refetch.Holdings.Count() == 3);
            Assert.IsTrue(refetch.Holdings[0].HoldingId > 0);
            Assert.IsTrue(refetch.Holdings[1].HoldingId > 0);
            Assert.IsTrue(refetch.Holdings[1].HoldingId > 0);

            // Clean-up (Act for Delete)
            existingClient.IsMarkedForDeletion = true;
            _clientRepo.Persist(existingClient);

            // Assert for Delete
            var result = _clientRepo.Fetch(testClientId);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void ClientRepository_InsertUpdateDelete()
        {
            // Arrange
            Client newClient = CreateSampleClient();

            // Act - Insert Client
            var existingClient = _clientRepo.Persist(newClient);
            var testClientId = existingClient.ClientId;

            // Act for Update
            // Modify each scalar property
            existingClient.Code = "XYZ";
            existingClient.FirstName = "Tom";
            existingClient.LastName = "Cruise";
            existingClient.Phone = "(323) 777 8888";
            existingClient.Address = "Beverly Hills, CA";
            existingClient.IsDirty = true;

            // Delete a Holding
            var holdingDeletedHoldingId = existingClient.Holdings[0].HoldingId;
            existingClient.Holdings[0].IsMarkedForDeletion = true;

            // Leave one Holding unchanged
            var holdingUnchangedHoldingId = existingClient.Holdings[1].HoldingId;

            // Modify one Holding
            var holdingModifiedHoldingId = existingClient.Holdings[2].HoldingId;
            existingClient.Holdings[2].Quantity = 888000;
            existingClient.Holdings[2].IsDirty = true;

            // Perform the update and refetch again
            // updatedItem and testShow refer to the same C# object
            existingClient = _clientRepo.Persist(existingClient);

            // re-fetching the same show from the database creates a
            // new C# object, that should be an exact replica of testClient
            var refetch = _clientRepo.Fetch(testClientId).FirstOrDefault();

            // Assert - Make sure updated item has proper flags set
            Assert.IsTrue(existingClient.IsDirty == false);
            Assert.IsTrue(existingClient.Code == "XYZ");

            // Assert - Make sure re-fetched Client has expected properties
            Assert.IsNotNull(refetch);
            Assert.IsTrue(refetch.IsDirty == false);
            Assert.IsTrue(refetch.IsMarkedForDeletion == false);
            Assert.IsTrue(refetch.Code == "XYZ");
            Assert.IsTrue(refetch.FirstName == "Tom");
            Assert.IsTrue(refetch.LastName == "Cruise");
            Assert.IsTrue(refetch.Phone == "(323) 777 8888");
            Assert.IsTrue(refetch.Address == "Beverly Hills, CA");
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
            existingClient.IsMarkedForDeletion = true;
            _clientRepo.Persist(existingClient);

            // Assert for Delete
            var result = _clientRepo.Fetch(testClientId);
            Assert.IsFalse(result.Any());
        }

        /*
        [TestMethod]
        public void ShowRepository_InvalidRating_ThrowsSqlException()
        {
            // Arrange
            Show newShow = CreateSampleShow();

            try
            {
                // Act - Insert Show with an invalid rating
                // (Doesn't refer to an existing person id).
                newShow.MpaaRatingId = -1;
                var existingShow = _showRepo.Persist(newShow);
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
        /*
            [TestMethod]
            public void ShowRepository_InvalidCredit_TransactionRollsBack()
            {
                // Arrange
                Show newShow = CreateSampleShow();

                // Act - Insert Show with a bad Cast member record
                // (Doesn't refer to an existing person id).
                newShow.Credits[0].PersonId = -1;
                try
                {
                    // Should throw exception
                    var existingShow = _showRepo.Persist(newShow);
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex, typeof(SqlException));
                }

                // Make sure parent show object was NOT saved.
                var savedShow = _showRepo.Fetch()
                    .Where(o => o.Title == "TestTitle")
                    .FirstOrDefault();
                Assert.IsNull(savedShow);
            }
            */

        #endregion

    }
}
