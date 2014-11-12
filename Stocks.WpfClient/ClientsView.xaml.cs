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

            Search();
        }

        private void Search()
        {
            var previouslySelectedItem = (Client)ResultsListBox.SelectedItem;
            _clients = new ObservableCollection<Client>(_clientRepository.Fetch());
            this.ResultsListBox.ItemsSource = _clients;
            Client selectedClient = null;
            if (previouslySelectedItem != null)
            {
                selectedClient = _clients
                .Where(o => o.ClientId == previouslySelectedItem.ClientId)
                .FirstOrDefault();
            }
            if (selectedClient != null)
            {
                ResultsListBox.SelectedItem = selectedClient;
            }
            else
            {
                ResultsListBox.SelectedIndex = 0;
            }
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
