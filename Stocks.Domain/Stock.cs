using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<Holding> _holdings = new ObservableCollection<Holding>();

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

        public ObservableCollection<Holding> Holdings
        {
            get { return _holdings; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return CompanyName + " " + Code + " " + LastPrice;
        }

        protected override bool GetHasChanges()
        {
            if (base.GetHasChanges())
                return true;
            foreach (var h in Holdings)
            {
                if (h.IsDirty || h.IsMarkedForDeletion)
                    return true;
            }

            return false;
        }

        public override string Validate(string propertyName = null)
        {
            List<string> errors = new List<string>();
            string err;
            switch (propertyName)
            {
                case "Code":
                    if (String.IsNullOrEmpty(Code))
                        errors.Add("Code is required.");
                    if (Code != null && Code.Length > 50)
                        errors.Add("Code cannot exceed 50 characters");
                    break;
                case "CompanyName":
                    if (String.IsNullOrEmpty(CompanyName))
                        errors.Add("Company Name is required.");
                    if (CompanyName != null && CompanyName.Length > 50)
                        errors.Add("Company Name cannot exceed 50 characters");
                    break;
                case "LastPrice":
                    if (LastPrice <= 0)
                        errors.Add("LastPrice must be a positive number");
                    break;
                case "Holdings":
                    foreach (var h in Holdings)
                    {
                        err = h.Validate();
                        if (err != null) errors.Add(err);
                    }
                    break;
                case null:
                    err = Validate("CompanyName");
                    if (err != null) errors.Add(err);

                    err = Validate("Code");
                    if (err != null) errors.Add(err);
                    
                    err = Validate("LastPrice");
                    if (err != null) errors.Add(err);

                    err = Validate("Holdings");
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
