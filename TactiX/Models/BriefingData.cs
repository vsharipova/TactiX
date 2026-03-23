using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TactiX.Models
{
    public class BriefingData
    {
        public double FieldWidth { get; set; } = 100.0;
        public double FieldHeight { get; set; } = 50.0; 
        public string DistanceUnit { get; set; } = "meters";
        public List<BriefingElement> Elements { get; set; } = new List<BriefingElement>();
    }

    public class BriefingElement
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Type { get; set; } 
        public double X { get; set; }
        public double Y { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double Rotation { get; set; } = 0;
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }
}