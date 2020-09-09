using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevCenterGallary.Common.Models
{
    public class Package
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonPropertyName("Id")]
        public string PackageId { get; set; }
        public string FileName { get; set; }
        public string PackageVersion { get; set; }
        public string Architecture { get; set; }
        public List<Asset> Assets { get; set; }
        [NotMapped]
        public List<TargetPlatform> RuntimeTargetPlatforms { get; set; }
        
        public FileInfo PackgeFileInfo { get; set; }

        public TargetPlatform TargetPlatform { get; set; }

        public PreinstallKitStatus PreinstallKitStatus { get; set; }
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
        public FileInfo FileInfo { get; set; }

    }

    public class FileInfo
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string SasUrl { get; set; }
    }

    public class TargetPlatform
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string MinVersion { get; set; }
        public string PlatformName { get; set; }

        public override string ToString() => $"{PlatformName} min version {MinVersion}";
    }
}
