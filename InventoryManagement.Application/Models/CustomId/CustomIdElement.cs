using System.Collections.Generic;

namespace InventoryManagement.Application.Models.CustomId
{
    // A class to hold the entire configuration
    public class CustomIdConfiguration
    {
        public List<CustomIdElement> Elements { get; set; } = new List<CustomIdElement>();
    }

    // A single element in the custom ID format
    public class CustomIdElement
    {
        public CustomIdElementType Type { get; set; }
        public string Value { get; set; } = string.Empty; // For Fixed Text
        public string Format { get; set; } = string.Empty; // For Date/Time, Numbers, etc.
    }

    public enum CustomIdElementType
    {
        FixedText,
        Sequence,
        RandomString,
        DateTime
    }
}