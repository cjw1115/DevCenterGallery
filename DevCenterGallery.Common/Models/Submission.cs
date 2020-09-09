using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DevCenterGallary.Common.Models
{
    public class Submission
    {
        [JsonIgnore]
        public int Id { get; set; }

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

        public List<Package> Packages { get; set; } 
    }
}
