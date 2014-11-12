using System;
using System.Collections.Generic;
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
        private List<Holding> _holdings = new List<Holding>();

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

        public List<Holding> Holdings
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

        #endregion
    }
}
