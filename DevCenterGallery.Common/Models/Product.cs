using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DevCenterGallary.Common.Models
{
    public class Product
    {
        [JsonPropertyName("bigId")]
        public string BigId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("logoUri")]
        public string LogoUri { get; set; }

        public IList<Submission> Submissions { get; set; }
    }
}
