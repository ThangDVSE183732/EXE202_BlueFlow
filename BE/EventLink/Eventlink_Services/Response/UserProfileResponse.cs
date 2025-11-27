using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Response
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyLogoUrl { get; set; }
        public string Industry { get; set; }
        public string CompanySize { get; set; }
        public int? FoundedYear { get; set; }
        public string CompanyDescription { get; set; }
        public string SocialProfile { get; set; }
        public string LinkedInProfile { get; set; }
        public string OfficialEmail { get; set; }
        public string StateProvince { get; set; }
        public string CountryRegion { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string DirectEmail { get; set; }
        public string DirectPhone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Role { get; set; }
    }
}
