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
using Stocks.DataAccess.Ado;
using Stocks.Domain;
using System.Collections.ObjectModel;

namespace Stocks.WpfClient
{
    /// <summary>
    /// Interaction logic for ClientsView.xaml
    /// </summary>

    public partial class ClientsView : UserControl
    {
        ClientRepository _clientRepository;
        ObservableCollection<Client> _clients;

        public ClientsView()
        {
            InitializeComponent();

            _clientRepository = new ClientRepository();
            _clients = new ObservableCollection<Client>();
            ResultsListBox.ItemsSource = _clients;

            //Search();
        }

        private void Search()
        {
            var previous = (Client)ResultsListBox.SelectedItem;
            _clients.Clear();
            /*
            ShowCriteria crit = new ShowCriteria
            {
                Title = this.TitleCriterion.Text,
                MpaaRatingId = (int)MpaaRatingCriterion.SelectedValue
            };
            */
            //foreach (var s in _clientRepository.Fetch(crit))
            foreach (var s in _clientRepository.Fetch())
            {
                _clients.Add(s);
            }

            // Find our way back to the previously selected
            // list entry by Id, if it's still there.
            ResultsListBox.SelectedItem =
                _clients.Where(s => previous != null
                    && s.ClientId == previous.ClientId)
                    .FirstOrDefault();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new Client();
            _clients.Add(newItem);
            ResultsListBox.SelectedItem = newItem;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (Client)ResultsListBox.SelectedItem;
            if (item == null) return;
            var msg = String.Format("Are you sure you want to delete client {0}?", 
                item.FirstLastName);
            if (MessageBox.Show(msg, "Confirm Delete?",
                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                item.IsMarkedForDeletion = true;
                _clientRepository.Persist(item);
                _clients.Remove(item);
            } 
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (Client)ResultsListBox.SelectedItem;
            if (item == null) return;
            _clientRepository.Persist(item);
            Search();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }
    }
}
