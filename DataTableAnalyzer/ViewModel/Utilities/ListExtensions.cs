using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DataTableAnalyzer.ViewModel.Utilities
{
    public static class ListExtensions
    {
        /// <summary>
        /// Is list of strings can be list of doubles?
        /// </summary>
        /// <param name="list">List of strings</param>
        /// <returns>True if list of strings can be list of doubles.</returns>
        public static bool IsNumberList(this List<string> list) {
            if (list.Count <= 0)
                return false;
            return list.TrueForAll((value) => 
                double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double kek));
        }

        /// <summary>
        /// Convert list of string to list of doubles.
        /// </summary>
        /// <param name="list">List of strings.</param>
        /// <returns>List of doubles</returns>
        public static List<double> StrToDouble(this List<string> list) {
            return list.Select((str) => 
                double.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture)).ToList();
        }

        /// <summary>
        /// Get list of column's values.
        /// </summary>
        /// <param name="columnNum"> Number of the chosen column. </param>
        /// <param name="table"> Data table. </param>
        /// <returns>List of strings.</returns>
        public static List<string> GetColumnRows(int columnNum, DataTable table) {
            List<string> resList = new List<string>();
            foreach (DataRow data in table.Rows) {
                resList.Add(data.ItemArray[columnNum].ToString());
            }

            return resList;
        }
    }
}
