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
            set { _stockId = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value ?? String.Empty; }
        }

        public string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value ?? String.Empty; }
        }

        public decimal LastPrice
        {
            get { return _lastPrice; }
            set { _lastPrice = value; }
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
