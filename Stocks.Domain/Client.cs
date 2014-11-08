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
            set { _clientId = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value ?? String.Empty; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value ?? String.Empty; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value ?? String.Empty; }
        }
        
        public string Phone
        {
            get { return _phone; }
            set { _phone = value ?? String.Empty; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value ?? String.Empty; }
        }

        public List<Holding> Holdings
        {
            get { return _holdings; }
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
