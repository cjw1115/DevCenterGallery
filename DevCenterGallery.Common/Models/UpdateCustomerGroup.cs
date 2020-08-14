using System.Collections.Generic;

namespace DevCenterGallary.Common.Models
{
    public class UpdateCustomerGroup
    {
        public List<string> Members { get; set; } = new List<string>();
        public string Name { get; set; }
        public string Type { get; set; } = "User";
    }
}
