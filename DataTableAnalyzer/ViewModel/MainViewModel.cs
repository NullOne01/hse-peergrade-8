using DataTableAnalyzer.View;
using DataTableAnalyzer.ViewModel.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataTableAnalyzer.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private DataTable selectedTable;
        public DataTable SelectedTable {
            get { return selectedTable; }
            set {
                selectedTable = value;
                OnPropertyChanged("SelectedTable");
            }
        }

        private RelayCommand<DataGrid> openCSVCommand;
        public RelayCommand<DataGrid> OpenCSVCommand {
            get {
                openCSVCommand = new RelayCommand<DataGrid>((grid) => OpenCSVFile(grid));
                return openCSVCommand;
            }
        }

        /// <summary>
        /// Open dialogue, read csv from file, fill table.
        /// </summary>
        /// <param name="dataGrid"> UI element of datatable. </param>
        public void OpenCSVFile(DataGrid dataGrid) {
            try {
                string newFilePath = OpenCSVDialog();
                if (newFilePath == string.Empty) {
                    return;
                }
                SelectedTable = TableLoader.ReadFileCSV(newFilePath);
                XSelectedItem = null;
                YSelectedItem = null;
                NumericItems.Clear();
                FillDataGrid(dataGrid);
            }
            catch (IOException) {
                MessageBox.Show("Процесс занят :c");
            }
            catch {
                MessageBox.Show("Чё-то не получилось :c");
            }
        }

        /// <summary>
        /// Open CSV dialog to open csv file.
        /// </summary>
        /// <returns>FilePath of opened file.</returns>
        private string OpenCSVDialog() {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV table (*.csv)|*.csv";
            if ((bool)openFileDialog.ShowDialog()) {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Fill UI dataGrid with DataTable.
        /// </summary>
        /// <param name="dataGrid"></param>
        private void FillDataGrid(DataGrid dataGrid) {
            // Add columns manually, to prevent bug 
            // (header's binding could have special chars like '/', '{'. the app could crush)

            dataGrid.Columns.Clear();
            for (int i = 0; i < SelectedTable.Columns.Count; i++) {
                var col = SelectedTable.Columns[i];
                DataGridTextColumn textColumn = new DataGridTextColumn
                {
                    Header = new TextBlock { Text = col.ToString() },
                    Binding = new Binding(string.Format("[{0}]", i))
                };

                AddContextMenuToColumn(textColumn, i);
                dataGrid.Columns.Add(textColumn);
            }

            dataGrid.ItemsSource = SelectedTable.Rows;
        }

        /// <summary>
        /// Add context menu options to the column's headers.
        /// </summary>
        /// <param name="textColumn">Column</param>
        /// <param name="columnNum">Number of the column.</param>
        private void AddContextMenuToColumn(DataGridTextColumn textColumn, int columnNum) {
            var cm = new ContextMenu();
            MenuItem uniqueColumnsItem = new MenuItem
            {
                Header = "Количественные данные (гистограмма)"
            };
            uniqueColumnsItem.Click += (o, e) => OpenUniqueColumns(columnNum);

            if (IsColumnNumeric(columnNum, out List<double> doubleValues)) {
                MenuItem columnInfoItem = new MenuItem
                {
                    Header = "Статистика (для числовых колонок)"
                };

                columnInfoItem.Click += (o, e) => OpenColumnInfo(columnNum, doubleValues);
                cm.Items.Add(columnInfoItem);
                // Add this column to the list of columns which are used for GraphWindow.
                NumericItems.Add(SelectedTable.Columns[columnNum]);
            }

            cm.Items.Add(uniqueColumnsItem);

            (textColumn.Header as TextBlock).ContextMenu = cm;
        }


        private RelayCommand openGraphCommand;
        public RelayCommand OpenGraphCommand {
            get {
                openGraphCommand = new RelayCommand(() => OpenGraph(),
                    () => XSelectedItem != null && YSelectedItem != null);
                return openGraphCommand;
            }
        }

        /// <summary>
        /// Value from ComboBox X.
        /// </summary>
        private DataColumn xSelectedItem;
        public DataColumn XSelectedItem {
            get => xSelectedItem;
            set {
                xSelectedItem = value;
                OnPropertyChanged("XSelectedItem");
            }
        }

        /// <summary>
        /// Value from ComboBox Y.
        /// </summary>
        private DataColumn ySelectedItem;
        public DataColumn YSelectedItem {
            get => ySelectedItem;
            set {
                ySelectedItem = value;
                OnPropertyChanged("YSelectedItem");
            }
        }

        public ObservableCollection<DataColumn> NumericItems { get; set; } = new ObservableCollection<DataColumn>();

        /// <summary>
        /// Open GraphWindow.
        /// </summary>
        private void OpenGraph() {
            List<double> xValues = ListExtensions.GetColumnRows(XSelectedItem.Ordinal, SelectedTable).StrToDouble();
            List<double> yValues = ListExtensions.GetColumnRows(YSelectedItem.Ordinal, SelectedTable).StrToDouble();
            GraphWindow graphWindow = new GraphWindow(xValues, yValues, XSelectedItem.ColumnName, YSelectedItem.ColumnName);
            graphWindow.Show();
        }

        /// <summary>
        /// Check if column has only numeric values. Returns list of numberic values.
        /// </summary>
        /// <param name="columnNum"> Number of the column. </param>
        /// <param name="values"> Return list of numberic values. </param>
        /// <returns> True if column is numeric. </returns>
        private bool IsColumnNumeric(int columnNum, out List<double> values) {
            List<string> valuesStr = ListExtensions.GetColumnRows(columnNum, SelectedTable);

            if (valuesStr.IsNumberList()) {
                values = valuesStr.StrToDouble();
                return true;
            }

            values = new List<double>();
            return false;
        }

        /// <summary>
        /// Open UniqueColumnsWindow.
        /// </summary>
        /// <param name="columnNum">Number of the column.</param>
        private void OpenUniqueColumns(int columnNum) {
            List<string> values = ListExtensions.GetColumnRows(columnNum, SelectedTable);

            UniqueColumnsWindow uniqueColumnsWindow =
                new UniqueColumnsWindow(values, SelectedTable.Columns[columnNum].ColumnName);
            uniqueColumnsWindow.Show();
        }

        /// <summary>
        /// Open ColumnInfo Window.
        /// </summary>
        /// <param name="columnNum">Number of the column.</param>
        /// <param name="values">Values to pass into ColumnInfo Window.</param>
        private void OpenColumnInfo(int columnNum, List<double> values) {
            ColumnInfoWindow columnInfoWindow =
                new ColumnInfoWindow(values, SelectedTable.Columns[columnNum].ColumnName);
            columnInfoWindow.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
