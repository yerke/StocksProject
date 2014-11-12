using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Ucla.Common.ExtensionMethods;

namespace Stocks.WpfClient.ExtensionMethods
{
    public static class WpfDataGridExtensions
    {
        /// <summary>
        /// Method to get a specific cell of a WPF DataGrid based on a reference to
        /// a bound object in the ItemsSource and an 0-based index of the column.
        /// 
        /// The cell returned by this method can be used, for example, to set focus on
        /// it:   cell.Focus();
        /// </summary>
        /// <param name="grd">WPF Data Grid reference</param>
        /// <param name="selectedItem">object reference to specific item in bound collection</param>
        /// <param name="columnIndex">Column position 0 = first column</param>
        /// <returns>A DataGridCell reprsenting the cell.</returns>
        public static DataGridCell FindCellByBoundItemAndColumnIndex(this DataGrid grd, Object selectedItem, int columnIndex)
        {
            DataGridCell cell = null;

            DataGridRow row = grd.ItemContainerGenerator.ContainerFromItem(selectedItem) as DataGridRow;
            // row might be null if the row is virtualized, then we need to try again
            // after scrolling it into view (and memory)
            if (row == null)
            {
                grd.ScrollIntoView(selectedItem);
                row = grd.ItemContainerGenerator.ContainerFromItem(selectedItem) as DataGridRow;
            }
            if (row != null)
            {
                DataGridCellsPresenter presenter = row.FindVisualChild<DataGridCellsPresenter>();
                // presenter might be null if the visual tree is virtualized,  then we need to
                // try again after applying the visual template to materialize the visual tree.
                if (presenter == null)
                {
                    // Materialize the row's visual tree
                    row.ApplyTemplate();
                    presenter = row.FindVisualChild<DataGridCellsPresenter>();
                }
                if (presenter != null)
                {
                    cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    if (cell == null)
                    {
                        grd.ScrollIntoView(grd.Columns[1]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    }
                }
            }
            return cell;

        }
    }
}
