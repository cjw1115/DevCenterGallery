﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevCenterGallary.Common.Models
{
    public class Package: ViewModelBase
    {
        [JsonPropertyName("Id")]
        public string PackageId { get; set; }
        public string FileName { get; set; }
        public string PackageVersion { get; set; }
        public string Architecture { get; set; }
        public IList<Asset> Assets { get; set; }
        public IList<TargetPlatform> RuntimeTargetPlatforms { get; set; }
        
        [JsonIgnore]
        public FileInfo PcakgeFileInfo { get; set; }

        [JsonIgnore]
        public TargetPlatform TargetPlatform { get; set; }

        private PreinstallKitStatus _preinstallKitStatus;
        [JsonIgnore]
        public PreinstallKitStatus PreinstallKitStatus
        {
            get => _preinstallKitStatus;
            set => Set(ref _preinstallKitStatus, value);
        }
    }

    public enum PreinstallKitStatus
    {
        Ready,
        Generating,
        NeedToGenerate
    }

    public class Asset
    {
        public string AssetType { get; set; }
        public FileInfo FileInfo { get; set; }

    }

    public class FileInfo
    {
        public string FileName { get; set; }
        public string SasUrl { get; set; }
    }

    public class TargetPlatform
    {
        public string MinVersion { get; set; }
        public string PlatformName { get; set; }

        public override string ToString() => $"{PlatformName} min version {MinVersion}";
    }
}