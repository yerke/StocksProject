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

        CommandBinding _newBinding;
        CommandBinding _deleteBinding;
        CommandBinding _saveBinding;
        CommandBinding _cancelBinding;
        CommandBinding _searchBinding;

        public StocksView()
        {
            InitializeComponent();

            _stockRepository = new StockRepository();
            _stocks = new ObservableCollection<Stock>();

            ResultsListBox.ItemsSource = _stocks;

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
            Stock selectedStock = ResultsListBox.SelectedItem as Stock;
            e.CanExecute = selectedStock == null
                ? true
                : !selectedStock.HasChanges;
        }

        private void Handle_New(object sender, ExecutedRoutedEventArgs e)
        {
            var newItem = new Stock();
            _stocks.Add(newItem);
            ResultsListBox.SelectedItem = newItem;
        }

        private void Handle_CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            Stock selectedStock = ResultsListBox.SelectedItem as Stock;
            e.CanExecute = selectedStock != null;
        }

        private void Handle_Delete(object sender, ExecutedRoutedEventArgs e)
        {
            var item = (Stock)ResultsListBox.SelectedItem;
            if (item == null) return;
            var msg = String.Format("Are you sure you want to delete {0} stock?", 
                item.CompanyName);
            if (MessageBox.Show(msg, "Confirm Delete?",
                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                item.IsMarkedForDeletion = true;
                _stockRepository.Persist(item);
                _stocks.Remove(item);
            }
        }

        private void Handle_CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            Stock selectedStock = ResultsListBox.SelectedItem as Stock;
            e.CanExecute = selectedStock == null
                ? false
                : selectedStock.HasChanges && selectedStock.Error == null;
                //: selectedStock.HasChanges;
            // Yerke's
            if (selectedStock != null && selectedStock.Error != null)
            {
                StatusBarTextBlock.Text = selectedStock.Error;
            }
            else
            {
                StatusBarTextBlock.Text = "";
            }
        }

        private void Handle_Save(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Stock selectedStock = ResultsListBox.SelectedItem as Stock;
                if (selectedStock == null) return;

                //// Yerke's part starts
                //var err = selectedStock.Error;
                //if (err != null)
                //{
                //    MessageBox.Show(err);
                //    return;
                //}
                //else
                //{
                //    MessageBox.Show("No problem.");
                //}
                //// Yerke's part ends

                _stockRepository.Persist(selectedStock);
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
            Stock selectedStock = ResultsListBox.SelectedItem as Stock;
            e.CanExecute = selectedStock == null
                ? false
                : selectedStock.HasChanges;
        }

        private void Handle_Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Search();
        }

        private void Handle_CanSearch(object sender, CanExecuteRoutedEventArgs e)
        {
            Stock selectedStock = ResultsListBox.SelectedItem as Stock;
            e.CanExecute = selectedStock == null || !selectedStock.HasChanges;
        }

        private void Handle_Search(object sender, ExecutedRoutedEventArgs e)
        {
            Search();
        }

        #endregion

        #region Private Methods

        private void Search()
        {
            var previous = (Stock)ResultsListBox.SelectedItem;
            _stocks.Clear();
            StockCriteria crit = new StockCriteria
            {
                Name = this.NameCriterion.Text
            };
            foreach (var s in _stockRepository.Fetch(crit))
            {
                _stocks.Add(s);
            }

            // Find our way back to the previously selected
            // list entry by Id, if it's still there.
            ResultsListBox.SelectedItem =
                _stocks.Where(s => previous != null
                    && s.StockId == previous.StockId)
                    .FirstOrDefault();
        }

        #endregion
    }
}
