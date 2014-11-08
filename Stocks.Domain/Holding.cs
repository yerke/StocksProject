using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocks.Domain
{
    public class Holding : DomainBase
    {
        #region Constructor
        #endregion

        #region Fields

        private int _holdingId;
        private int _clientId;
        private int _stockId;
        private Int64 _quantity;
        private DateTime _lastChangeDate;

        #endregion

        #region Properties

        public int HoldingId
        {
            get { return _holdingId; }
            set { _holdingId = value; }
        }

        public int ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }

        public int StockId
        {
            get { return _stockId; }
            set { _stockId = value; }
        }

        public Int64 Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public DateTime LastChangeDate
        {
            get { return _lastChangeDate; }
            set { _lastChangeDate = value; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return Quantity.ToString() + " pcs of " + StockId.ToString() + 
                " in portfolio " + ClientId.ToString();
        }

        #endregion
    }
}
