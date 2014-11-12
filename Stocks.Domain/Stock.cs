using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocks.Domain
{
    public class Stock : DomainBase
    {
        #region Constructor
        #endregion

        #region Fields

        private int _stockId;
        private string _code = String.Empty;
        private string _companyName = String.Empty;
        private decimal _lastPrice;
        private List<Holding> _holdings = new List<Holding>();

        #endregion

        #region Properties

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

        public string Code
        {
            get { return _code; }
            set 
            { 
                if (_code == (value ?? String.Empty)) return;
                _code = value ?? String.Empty;
                OnPropertyChanged();
            }
        }

        public string CompanyName
        {
            get { return _companyName; }
            set 
            { 
                if (_companyName == (value ?? String.Empty)) return;
                _companyName = value ?? String.Empty;
                OnPropertyChanged();
            }
        }

        public decimal LastPrice
        {
            get { return _lastPrice; }
            set 
            { 
                if (_lastPrice == value) return;
                _lastPrice = value;
                OnPropertyChanged();
            }
        }

        public List<Holding> Holdings
        {
            get { return _holdings; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return CompanyName + " " + Code + " " + LastPrice;
        }

        #endregion
    }
}
