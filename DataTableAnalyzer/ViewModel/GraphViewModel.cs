using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

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
                    Title = "Линия"
                }
            };
            ObservablePoint[] observablePoints = new ObservablePoint[Math.Min(xValues.Count, yValues.Count)];
            ChartValues<ObservablePoint> points = new ChartValues<ObservablePoint>();
            for (int i = 0; i < Math.Min(xValues.Count, yValues.Count); i++) {
                observablePoints[i] = new ObservablePoint(XValues[i], YValues[i]);
            }
            points.AddRange(observablePoints);
            SeriesCollection[0].Values = points;
        }
    }
}
