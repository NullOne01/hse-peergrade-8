using DataTableAnalyzer.View;
using DataTableAnalyzer.ViewModel.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

        public void OpenCSVFile(DataGrid dataGrid) {
            string newFilePath = OpenCSVDialog();
            if (newFilePath == string.Empty) {
                return;
            }
            try {
                SelectedTable = TableLoader.ReadFileCSV(newFilePath);
                FillDataGrid(dataGrid);
            }
            catch (IOException) {
                MessageBox.Show("Процесс занят :c");
            }
            catch {
                MessageBox.Show("Чё-то не получилось :c");
            }
        }

        private string OpenCSVDialog() {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV table (*.csv)|*.csv";
            if ((bool)openFileDialog.ShowDialog()) {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

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
            }

            cm.Items.Add(uniqueColumnsItem);

            (textColumn.Header as TextBlock).ContextMenu = cm;
        }

        private bool IsColumnNumeric(int columnNum, out List<double> values) {
            List<string> valuesStr = new List<string>();
            foreach (DataRow data in SelectedTable.Rows) {
                valuesStr.Add(data.ItemArray[columnNum].ToString());
            }

            if (valuesStr.IsNumberList()) {
                values = valuesStr.StrToDouble();
                return true;
            }

            values = new List<double>();
            return false;
        }

        private void OpenUniqueColumns(int columnNum) {
            List<string> values = new List<string>();
            foreach (DataRow data in SelectedTable.Rows) {
                values.Add(data.ItemArray[columnNum].ToString());
            }

            UniqueColumnsWindow uniqueColumnsWindow = 
                new UniqueColumnsWindow(values, SelectedTable.Columns[columnNum].ColumnName);
            uniqueColumnsWindow.Show();
        }

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
