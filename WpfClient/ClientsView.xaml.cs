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

namespace WpfClient
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

            foreach (var s in _clientRepository.Fetch(null))
            {
                _clients.Add(s);
            }

            ResultsListBox.ItemsSource = _clients;
        }
    }
}
