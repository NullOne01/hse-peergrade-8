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
        public static bool IsNumberList(this List<string> list) {
            return list.TrueForAll((value) => 
                double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double kek));
        }

        public static List<double> StrToDouble(this List<string> list) {
            return list.Select((str) => 
                double.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture)).ToList();
        }

        public static List<string> GetColumnRows(int columnNum, DataTable table) {
            List<string> resList = new List<string>();
            foreach (DataRow data in table.Rows) {
                resList.Add(data.ItemArray[columnNum].ToString());
            }

            return resList;
        }
    }
}
