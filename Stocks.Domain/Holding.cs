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

        public Holding()
        {
            LastChangeDate = DateTime.Now.Date; 
            // So it will not set to 1/1/0001 by default at creation
        }

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
            set 
            {
                if (_holdingId == value) return;
                _holdingId = value;
                OnPropertyChanged();
            }
        }

        public int ClientId
        {
            get { return _clientId; }
            set 
            { 
                if (_clientId == value) return;
                _clientId = value;
                OnPropertyChanged();
            }
        }

        public int StockId
        {
            get { return _stockId; }
            set 
            { 
                if (_stockId == value) return;
                _stockId = value;
                OnPropertyChanged();
            }
        }

        public Int64 Quantity
        {
            get { return _quantity; }
            set 
            { 
                if (_quantity == value) return;
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastChangeDate
        {
            get { return _lastChangeDate; }
            set 
            {
                if (_lastChangeDate == value) return;
                _lastChangeDate = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return Quantity.ToString() + " pcs of " + StockId.ToString() + 
                " in portfolio " + ClientId.ToString();
        }

        public override string Validate(string propertyName = null)
        {
            List<string> errors = new List<string>();
            string err;
            switch (propertyName)
            {
                // need to add StockId validation when run from Client 
                // and vice versa?
                case "Quantity":
                    if ((Quantity <= 0) || (Quantity % 1 != 0))
                        errors.Add("Quantity must be a positive integer number");
                    break;
                case "LastChangeDate":
                    if (LastChangeDate < new DateTime(1900, 1, 1) || 
                        LastChangeDate > DateTime.Now.Date)
                        errors.Add("LastChangeDate cannot be earlier than 1/1/1900" +
                            " or later than today's date");
                    break;
                case null:
                    err = Validate("Quantity");
                    if (err != null) errors.Add(err);

                    err = Validate("LastChangeDate");
                    if (err != null) errors.Add(err);
                    break;
                default:
                    return null;
            }
            return errors.Count == 0 ? null : String.Join("\r\n", errors);
        }

        #endregion
    }
}
