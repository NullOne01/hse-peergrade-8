using DataTableAnalyzer.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTableAnalyzer.ViewModel
{
    class ColumnInfoViewModel
    {
        public string ColumnName { get; set; }
        public double MeanNum { get; set; }
        public double MedianNum { get; set; }
        public double RootMeanNum { get; set; }
        public double DispersionNum { get; set; }

        public ColumnInfoViewModel(List<double> columnValues, string columnName) {
            ColumnName = columnName;
            MeanNum = columnValues.Average();
            MedianNum = (double)columnValues.Median();
            RootMeanNum = columnValues.StandardDeviation();
            DispersionNum = columnValues.Dispersion();
        }

    }
}
