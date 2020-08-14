using System.Text.Json.Serialization;
using Windows.UI.Xaml.Media;

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
        
        [JsonIgnore]
        public ImageSource ImageSource { get; set; }
    }
}
