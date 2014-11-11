using Stocks.DataAccess.Ado;
using Stocks.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stocks.WpfClient
{
    /// <summary>
    /// Interaction logic for StocksView.xaml
    /// </summary>
    public partial class StocksView : UserControl
    {
        StockRepository _stockRepository;
        ObservableCollection<Stock> _stocks;

        public StocksView()
        {
            InitializeComponent();

            _stockRepository = new StockRepository();
            _stocks = new ObservableCollection<Stock>();

            foreach (var s in _stockRepository.Fetch(null))
            {
                _stocks.Add(s);
            }

            ResultsListBox.ItemsSource = _stocks;
        }
    }
}
