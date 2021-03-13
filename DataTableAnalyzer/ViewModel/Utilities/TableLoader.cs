using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace DataTableAnalyzer.ViewModel
{
    class TableLoader
    {
        public static DataTable ReadFileCSV(string filepath) {
            DataTable dt = new DataTable();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null
            };

            // Process can be used. Possible bug.
            using (StreamReader file = new StreamReader(filepath)) {
                using var csv = new CsvReader(file, config);
                using var dr = new CsvDataReader(csv);
                dt.Load(dr);
            }

            return dt;
        }
    }
}
