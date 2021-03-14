using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using DataTableAnalyzer.ViewModel.Utilities;
using LiveCharts;
using LiveCharts.Wpf;

namespace DataTableAnalyzer.ViewModel
{
    class UniqueColumnsViewModel : INotifyPropertyChanged
    {
        public SeriesCollection SeriesCollection { get; set; }

        private string[] labels;
        public string[] Labels {
            get => labels;
            set {
                labels = value;
                OnPropertyChanged("Labels");
            }
        }

        public Func<int, string> Formatter { get; set; }
        private List<string> ColumnValues { get; set; } = new List<string>();
        private string ColumnName { get; set; }
        private Dictionary<string, int> ColumnValuesDict { get; set; } = new Dictionary<string, int>();

        public UniqueColumnsViewModel(List<string> columnValues, string columnName) {
            ColumnValues = columnValues;
            ColumnName = columnName;
            if (ColumnValues.IsNumberList()) {
                SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = ColumnName
                    }
                };
                SetUpNumericColumns();
                return;
            }

            CountUniqueValues();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = ColumnName,
                    Values = new ChartValues<int>(ColumnValuesDict.Values.ToList())
                }
            };

            Labels = ColumnValuesDict.Keys.ToArray();
        }


        private void CountUniqueValues() {
            foreach (var data in ColumnValues) {
                if (!ColumnValuesDict.ContainsKey(data))
                    ColumnValuesDict.Add(data, 0);
                ColumnValuesDict[data]++;
            }
        }

        public Visibility ShouldNumericShow {
            get {
                return ColumnValues.IsNumberList() ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SetUpNumericColumns() {
            // R.I.P. perfomance.
            List<double> values = ColumnValues.StrToDouble();

            double min = values.Min();
            double max = values.Max();
            int[] chartValuesCounter = new int[NumericColumnCount];
            string[] newLabels = new string[NumericColumnCount];

            double partLength = (max - min) / NumericColumnCount;
            for (int i = 0; i < NumericColumnCount; i++) {
                foreach (double value in values) {
                    bool shouldBeAdded = value >= min + partLength * i && value < min + partLength * (i + 1);
                    // If the last iteraction, then we should take take the max.
                    if (i == NumericColumnCount - 1) {
                        shouldBeAdded = value >= min + partLength * i && value <= min + partLength * (i + 1);
                        newLabels[i] = $"[{min + partLength * i:F3}; {min + partLength * (i + 1):F3}]";
                    } else {
                        newLabels[i] = $"[{min + partLength * i:F3}; {min + partLength * (i + 1):F3})";
                    }

                    if (shouldBeAdded) {
                        chartValuesCounter[i]++;
                    }
                }
            }

            SeriesCollection[0].Values = new ChartValues<int>(chartValuesCounter);
            Labels = newLabels;
        }

        private int numericColumnCount = 5;
        public int NumericColumnCount {
            get => numericColumnCount;
            set {
                numericColumnCount = value;
                SetUpNumericColumns();
                OnPropertyChanged("NumericColumnCount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
