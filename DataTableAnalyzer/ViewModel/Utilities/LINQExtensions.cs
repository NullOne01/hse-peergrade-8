using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTableAnalyzer.ViewModel.Utilities
{
    /// <summary>
    /// Class of LINQ expressions. Everything is copied from StackOverflow ¯\_(ツ)_/¯
    /// </summary>
    public static class LINQExtensions
    {
        public static double? Median<TColl, TValue>(this IEnumerable<TColl> source,
            Func<TColl, TValue> selector) {
            return source.Select<TColl, TValue>(selector).Median();
        }

        public static double? Median<T>(
            this IEnumerable<T> source) {
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
                source = source.Where(x => x != null);

            int count = source.Count();
            if (count == 0)
                return null;

            source = source.OrderBy(n => n);

            int midpoint = count / 2;
            if (count % 2 == 0)
                return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
            else
                return Convert.ToDouble(source.ElementAt(midpoint));
        }

        public static double StandardDeviation(this IEnumerable<double> values) {
            return Math.Sqrt(values.Dispersion());
        }

        public static double Dispersion(this IEnumerable<double> values) {
            double avg = values.Average();
            return values.Average(v => Math.Pow(v - avg, 2));
        }
    }
}
