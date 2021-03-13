using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataTableAnalyzer.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private DataGrid MainGrid { get; set; }
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
            MainGrid = dataGrid;
            //MessageBox.Show("Button clicked!");
            //SelectedTable = TableLoader.ReadFileCSV(@"F:\Projects\C#\hse-peergrade-8\task\Задание 8. Файлы\coursea_data.csv");
            SelectedTable = TableLoader.ReadFileCSV(@"F:\Projects\C#\hse-peergrade-8\task\Задание 8. Файлы\test1.csv");
            FillDataGrid();
        }

        private void FillDataGrid() {
            // Add columns manually, to prevent bug 
            // (header's binding could have special chars like '/', '{'. the app could crush)

            MainGrid.Columns.Clear();
            for (int i = 0; i < SelectedTable.Columns.Count; i++) {
                var col = SelectedTable.Columns[i];
                DataGridTextColumn textColumn = new DataGridTextColumn
                {
                    // Header of the column keeps removing first underscore. That sucks. (Fixed)
                    Header = col.ToString().Replace("_", "__"),
                    Binding = new Binding(string.Format("[{0}]", i))
                };

                MainGrid.Columns.Add(textColumn);
            }

            MainGrid.ItemsSource = SelectedTable.Rows;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
