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

        CommandBinding _newBinding;
        CommandBinding _deleteBinding;
        CommandBinding _saveBinding;
        CommandBinding _cancelBinding;
        CommandBinding _searchBinding;

        public ClientsView()
        {
            InitializeComponent();

            _clientRepository = new ClientRepository();
            _clients = new ObservableCollection<Client>();
            ResultsListBox.ItemsSource = _clients;

            //Search();

            _newBinding = new CommandBinding(ApplicationCommands.New);
            _newBinding.CanExecute += Handle_CanNew;
            _newBinding.Executed += Handle_New;
            CommandBindings.Add(_newBinding);

            _deleteBinding = new CommandBinding(ApplicationCommands.Delete);
            _deleteBinding.CanExecute += Handle_CanDelete;
            _deleteBinding.Executed += Handle_Delete;
            CommandBindings.Add(_deleteBinding);

            _saveBinding = new CommandBinding(ApplicationCommands.Save);
            _saveBinding.CanExecute += Handle_CanSave;
            _saveBinding.Executed += Handle_Save;
            CommandBindings.Add(_saveBinding);

            _cancelBinding = new CommandBinding(CustomCommands.Cancel);
            _cancelBinding.CanExecute += Handle_CanCancel;
            _cancelBinding.Executed += Handle_Cancel;
            CommandBindings.Add(_cancelBinding);

            _searchBinding = new CommandBinding(ApplicationCommands.Find);
            _searchBinding.CanExecute += Handle_CanSearch;
            _searchBinding.Executed += Handle_Search;
            CommandBindings.Add(_searchBinding);
        }

        #region Command Implementations

        private void Handle_CanNew(object sender, CanExecuteRoutedEventArgs e)
        {
            Client selectedClient = ResultsListBox.SelectedItem as Client;
            e.CanExecute = selectedClient == null
                ? true
                : !selectedClient.HasChanges;
        }

        private void Handle_New(object sender, ExecutedRoutedEventArgs e)
        {
            var newItem = new Client();
            _clients.Add(newItem);
            ResultsListBox.SelectedItem = newItem;
        }

        private void Handle_CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            Client selectedClient = ResultsListBox.SelectedItem as Client;
            e.CanExecute = selectedClient != null;
        }

        private void Handle_Delete(object sender, ExecutedRoutedEventArgs e)
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

        private void Handle_CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            Client selectedClient = ResultsListBox.SelectedItem as Client;
            e.CanExecute = selectedClient == null
                ? false
                : selectedClient.HasChanges && selectedClient.Error == null;
                //: selectedClient.HasChanges;
        }

        private void Handle_Save(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Client selectedClient = ResultsListBox.SelectedItem as Client;
                if (selectedClient == null) return;
                _clientRepository.Persist(selectedClient);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Save Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Search();
        }

        private void Handle_CanCancel(object sender, CanExecuteRoutedEventArgs e)
        {
            Client selectedClient = ResultsListBox.SelectedItem as Client;
            e.CanExecute = selectedClient == null
                ? false
                : selectedClient.HasChanges;
        }

        private void Handle_Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Search();
        }

        private void Handle_CanSearch(object sender, CanExecuteRoutedEventArgs e)
        {
            Client selectedClient = ResultsListBox.SelectedItem as Client;
            e.CanExecute = selectedClient == null || !selectedClient.HasChanges;
        }

        private void Handle_Search(object sender, ExecutedRoutedEventArgs e)
        {
            Search();
        }

        #endregion

        #region Private Methods

        private void Search()
        {
            var previous = (Client)ResultsListBox.SelectedItem;
            _clients.Clear();
            ClientCriteria crit = new ClientCriteria
            {
                Name = this.NameCriterion.Text
            };
            foreach (var s in _clientRepository.Fetch(crit))
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

        #endregion
    }
}
