using System;
using System.Collections.Generic;
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
using Stocks.Domain;
using Stocks.WpfClient.ExtensionMethods;

namespace Stocks.WpfClient
{
    /// <summary>
    /// Interaction logic for StockView.xaml
    /// </summary>
    public partial class StockView : UserControl
    {
        public StockView()
        {
            InitializeComponent();

            ClientCodeDropDownColumn.ItemsSource = LookupCache.Clients;
        }

        private void AddHoldingButton_Click(object sender, RoutedEventArgs e)
        {
            var stock = DataContext as Stock;
            var holding = new Holding();
            stock.Holdings.Add(holding);

            var cell = HoldingsDataGrid.FindCellByBoundItemAndColumnIndex(holding, 0);
            if (cell != null)
            {
                cell.Focus();
                cell.IsEditing = true;
            }
        }
    }
}
