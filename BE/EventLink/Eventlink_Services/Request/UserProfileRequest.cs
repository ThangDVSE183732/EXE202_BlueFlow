using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Eventlink_Services.Request
{
    public class CreateUserProfileRequest
    {
        // Company Information
        public string CompanyName { get; set; }
        public IFormFile? CompanyLogoUrl { get; set; }
        public string Industry { get; set; }
        public string CompanySize { get; set; }
        public int? FoundedYear { get; set; }
        public string CompanyDescription { get; set; }
        public string SocialProfile { get; set; }
        public string LinkedInProfile { get; set; }

        // Contact Information
        public string OfficialEmail { get; set; }
        public string StateProvince { get; set; }
        public string CountryRegion { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }

        // Primary Contact Person
        public string ContactFullName { get; set; }
        public string JobTitle { get; set; }
        public string DirectEmail { get; set; }
        public string DirectPhone { get; set; }
    }

    public class UpdateUserProfileRequest : CreateUserProfileRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
