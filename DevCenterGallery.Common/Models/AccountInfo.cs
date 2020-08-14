using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevCenterGallary.Common.Models
{
    public class AccountInfo
    {
        public class ControlInfo
        {
            [JsonPropertyName("identityType")]
            public string IdentityType { get; set; }
            [JsonPropertyName("isSignedIn")]
            public bool IsSignedIn { get; set; } = false;
            [JsonPropertyName("memberEmail")]
            public string MemberEmail { get; set; }

        }

        [JsonPropertyName("userId")]
        public uint UserId { get; set; }
        [JsonPropertyName("userEmail")]
        public string EMail { get; set; }
        [JsonPropertyName("meControl")]
        public ControlInfo MeControl { get; set; }
    }
}
