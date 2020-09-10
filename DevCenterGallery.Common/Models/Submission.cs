using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DevCenterGalley.Common.Models
{
    public class Submission
    {
        [Key]
        [JsonPropertyName("id")]
        public string SubmissionId { get; set; }

        [JsonPropertyName("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonPropertyName("publishedDateTime")]
        public DateTime PublishedDateTime { get; set; }

        [JsonPropertyName("updatedDateTime")]
        public DateTime UpdatedDateTime { get; set; }

        [JsonPropertyName("releaseRank")]
        public int ReleaseRank { get; set; }
        [Required]
        public List<Package> Packages { get; set; }

        [Required]
        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
