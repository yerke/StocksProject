using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocks.Domain
{
    public class Client : DomainBase
    {
        #region Constructor
        #endregion

        #region Fields

        private int _clientId;
        private string _code = String.Empty;
        private string _firstName = String.Empty;
        private string _lastName = String.Empty;
        private string _phone = String.Empty;
        private string _address = String.Empty;
        private ObservableCollection<Holding> _holdings = new ObservableCollection<Holding>();

        #endregion

        #region Properties

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

        public string FirstName
        {
            get { return _firstName; }
            set 
            { 
                if (_firstName == (value ?? String.Empty)) return;
                _firstName = value ?? String.Empty;
                OnPropertyChanged();
                OnPropertyChanged("FirstLastName");
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set 
            {
                if (_lastName == (value ?? String.Empty)) return;
                _lastName = value ?? String.Empty;
                OnPropertyChanged();
                OnPropertyChanged("FirstLastName");
            }
        }
        
        public string Phone
        {
            get { return _phone; }
            set 
            { 
                if (_phone == (value ?? String.Empty)) return;
                _phone = value ?? String.Empty;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get { return _address; }
            set 
            { 
                if (_address == (value ?? String.Empty)) return;
                _address = value ?? String.Empty;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Holding> Holdings
        {
            get { return _holdings; }
        }

        #endregion

        #region Computed Properties

        public string FirstLastName
        {
            get
            {
                return (FirstName + " " + LastName).Trim();
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return FirstName + " " + LastName + " " + Code;
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
                        errors.Add("Code cannot exceed 50 characters.");
                    break;
                case "FirstName":
                    if (String.IsNullOrEmpty(FirstName))
                        errors.Add("First Name is required.");
                    if (FirstName != null && FirstName.Length > 50)
                        errors.Add("Last Name cannot exceed 50 characters.");
                    break;
                case "LastName":
                    if (String.IsNullOrEmpty(LastName))
                        errors.Add("Last Name is required.");
                    if (LastName != null && LastName.Length > 50)
                        errors.Add("Last Name cannot exceed 50 characters.");
                    break;
                case "Phone":
                    if (String.IsNullOrEmpty(Phone))
                        errors.Add("Phone is required.");
                    if (Phone != null && Phone.Length > 50)
                        errors.Add("Phone cannot exceed 50 characters.");
                    break;
                case "Address":
                    if (String.IsNullOrEmpty(Address))
                        errors.Add("Address is required.");
                    if (Address != null && Address.Length > 100)
                        errors.Add("Address cannot exceed 100 characters.");
                    break;
                case "Holdings":
                    foreach (var h in Holdings)
                    {
                        err = h.Validate();
                        if (err != null) errors.Add(err);
                    }
                    break;
                case null:
                    err = Validate("Code");
                    if (err != null) errors.Add(err);

                    err = Validate("FirstName");
                    if (err != null) errors.Add(err);

                    err = Validate("LastName");
                    if (err != null) errors.Add(err);

                    err = Validate("Phone");
                    if (err != null) errors.Add(err);

                    err = Validate("Address");
                    if (err != null) errors.Add(err);

                    err = Validate("Holdings");
                    if (err != null) errors.Add(err);

                    break;
                default:
                    return null;
            }
            return errors.Count == 0 ? null : String.Join(" ", errors);
        }

        #endregion
    }
}
