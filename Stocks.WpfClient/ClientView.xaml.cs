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
    /// Interaction logic for ClientView.xaml
    /// </summary>
    public partial class ClientView : UserControl
    {
        public ClientView()
        {
            InitializeComponent();

            StockCodeDropDownColumn.ItemsSource = LookupCache.Stocks;
        }

        private void AddHoldingButton_Click(object sender, RoutedEventArgs e)
        {
            var client = DataContext as Client;
            var holding = new Holding();
            client.Holdings.Add(holding);

            var cell = HoldingsDataGrid.FindCellByBoundItemAndColumnIndex(holding, 0);
            if (cell != null)
            {
                cell.Focus();
                cell.IsEditing = true;
            }
        }
    }
}
