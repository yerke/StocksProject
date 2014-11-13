using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stocks.Domain;
using Stocks.DataAccess.Ado;

namespace Stocks.WpfClient
{
    public static class LookupCache
    {
        private static ClientRepository _clientRepository;
        private static StockRepository _stockRepository;
        private static IEnumerable<Client> _clients;
        private static IEnumerable<Stock> _stocks;

        public static IEnumerable<Client> Clients
        {
            get
            {
                if (_clients == null)
                {
                    if (_clientRepository == null)
                    {
                        _clientRepository = new ClientRepository();
                    }
                    _clients = _clientRepository.Fetch()
                        .OrderBy(o => o.LastName)
                        .ThenBy(o => o.FirstName).ToList();
                }
                return _clients;
            }
            set { _clients = value; }
        }

        public static IEnumerable<Stock> Stocks
        {
            get
            {
                if (_stocks == null)
                {
                    if (_stockRepository == null)
                    {
                        _stockRepository = new StockRepository();
                    }
                    _stocks = _stockRepository.Fetch()
                        .OrderBy(o => o.CompanyName).ToList();
                }
                return _stocks;
            }
            set { _stocks = value; }
        }

        public static void ClearCache()
        {
            _clients = null;
            _stocks = null;
        }
    }
}
