using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace DevCenterGallary.Common.Models
{
    public class Product
    {
        [Key]
        [JsonPropertyName("bigId")]
        public string BigId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("logoUri")]
        public string LogoUri { get; set; }

        [Required]
        public List<Submission> Submissions { get; set; }
    }
}
