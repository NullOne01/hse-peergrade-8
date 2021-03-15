using DataTableAnalyzer.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataTableAnalyzer.View
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public GraphWindow(List<double> xValues, List<double> yValues, string xLabel, string yLabel) {
            InitializeComponent();

            DataContext = new GraphViewModel(xValues, yValues, xLabel, yLabel);
        }
    }
}
