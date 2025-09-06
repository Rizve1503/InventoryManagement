using System.Collections.Generic;

namespace InventoryManagement.WebApp.ViewModels
{
    // Holds the stats for a single numeric field
    public class NumericStat
    {
        public string FieldName { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }

    // Holds the stats for a single string field
    public class StringStat
    {
        public string FieldName { get; set; } = string.Empty;
        public string MostFrequentValue { get; set; } = "N/A";
        public int Frequency { get; set; }
        public Dictionary<string, int> TopValues { get; set; } = new Dictionary<string, int>();

    }

    // The main ViewModel for the entire stats page
    public class InventoryStatsViewModel
    {
        public int TotalItems { get; set; }
        public List<NumericStat> NumericStats { get; set; } = new List<NumericStat>();
        public List<StringStat> StringStats { get; set; } = new List<StringStat>();
    }
}