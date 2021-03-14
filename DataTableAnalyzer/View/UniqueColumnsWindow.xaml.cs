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
    /// Interaction logic for UniqueColumnsWindow.xaml
    /// </summary>
    public partial class UniqueColumnsWindow : Window
    {
        public UniqueColumnsWindow(List<string> columnValues, string columnName) {
            InitializeComponent();

            DataContext = new UniqueColumnsViewModel(columnValues, columnName);
        }
    }
}
