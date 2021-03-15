using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DataTableAnalyzer.ViewModel
{
    class GraphViewModel
    {
        public SeriesCollection SeriesCollection { get; set; }

        public List<double> XValues { get; set; }
        public List<double> YValues { get; set; }
        public string XLabel { get; set; }
        public string YLabel { get; set; }

        public GraphViewModel(List<double> xValues, List<double> yValues, string xLabel, string yLabel) {
            XLabel = xLabel;
            YLabel = yLabel;
            XValues = xValues;
            YValues = yValues;

            SeriesCollection = new SeriesCollection
            {
                new LineSeries{
                    Title = $"{YLabel}({XLabel})"
                }
            };
            int pointsCount = Math.Min(xValues.Count, yValues.Count);
            if (pointsCount >= 1500)
                MessageBox.Show("Вы собираетесь построить график на >=1500 строк. Вы обрекли себя на долгое ожидание, я не виноват");

            ObservablePoint[] observablePoints = new ObservablePoint[pointsCount];
            ChartValues<ObservablePoint> points = new ChartValues<ObservablePoint>();
            for (int i = 0; i < pointsCount; i++) {
                observablePoints[i] = new ObservablePoint(XValues[i], YValues[i]);
            }
            points.AddRange(observablePoints);
            SeriesCollection[0].Values = points;
        }
    }
}
