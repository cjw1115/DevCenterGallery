using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevCenterGalley.Common.Models
{
    public class Package
    {
        [Key]
        [JsonPropertyName("Id")]
        public string PackageId { get; set; }
        public string FileName { get; set; }
        public string PackageVersion { get; set; }
        public string Architecture { get; set; }
        [Required]
        public List<Asset> Assets { get; set; }
        [Required]
        public List<TargetPlatform> RuntimeTargetPlatforms { get; set; }

        [JsonIgnore]
        [NotMapped]
        public FileInfo PackgeFileInfo { get; set; }
        [JsonIgnore]
        [NotMapped]
        public TargetPlatform TargetPlatform { get; set; }
        [JsonIgnore]
        [NotMapped]
        public PreinstallKitStatus PreinstallKitStatus { get; set; }
        [Required]
        [JsonIgnore]
        public virtual Submission Submission { get; set; }
    }

    public enum PreinstallKitStatus
    {
        Ready,
        Generating,
        NeedToGenerate
    }

    public class Asset
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string AssetType { get; set; }
        [Required]
        public FileInfo FileInfo { get; set; }

        [Required]
        [JsonIgnore]
        public virtual Package Package { get; set; }
    }

    public class FileInfo
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string SasUrl { get; set; }

        [Required]
        [JsonIgnore]
        public int AssetId { get; set; }
        [Required]
        [JsonIgnore]
        public Asset Asset { get; set; }
    }

    public class TargetPlatform
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string MinVersion { get; set; }
        public string PlatformName { get; set; }

        [Required]
        [JsonIgnore]
        public virtual Package Package { get; set; }

        public override string ToString() => $"{PlatformName} min version {MinVersion}";
    }
}
