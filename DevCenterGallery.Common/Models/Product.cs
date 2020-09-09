using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace DevCenterGallary.Common.Models
{
    public class Product
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonPropertyName("bigId")]
        public string BigId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("logoUri")]
        public string LogoUri { get; set; }

        public List<Submission> Submissions { get; set; }
    }
}
