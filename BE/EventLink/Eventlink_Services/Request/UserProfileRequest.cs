using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class UserProfileRequest
    {
        public class CreateUserProfileRequest
        {
            [Required(ErrorMessage = "Bio is required")]
            [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
            public string Bio { get; set; }

            [Required(ErrorMessage = "Company name is required")]
            [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
            public string CompanyName { get; set; }

            [Url(ErrorMessage = "Website must be a valid URL")]
            public string Website { get; set; }

            [Required(ErrorMessage = "Location is required")]
            [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
            public string Location { get; set; }

            [Url(ErrorMessage = "Profile image must be a valid URL")]
            public string ProfileImageUrl { get; set; }

            [Url(ErrorMessage = "Cover image must be a valid URL")]
            public string CoverImageUrl { get; set; }

            [Url(ErrorMessage = "LinkedIn must be a valid URL")]
            public string LinkedInUrl { get; set; }

            [Url(ErrorMessage = "Facebook must be a valid URL")]
            public string FacebookUrl { get; set; }

            public string PortfolioImages { get; set; }

            public string WorkSamples { get; set; }

            public string Certifications { get; set; }

            [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
            public int? YearsOfExperience { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Total projects completed must be a positive number")]
            public int? TotalProjectsCompleted { get; set; }

            [Range(0, 5, ErrorMessage = "Average rating must be between 0 and 5")]
            public decimal? AverageRating { get; set; }

            public string VerificationDocuments { get; set; }
        }

        public class UpdateUserProfileRequest
        {
            [Required(ErrorMessage = "Bio is required")]
            [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
            public string Bio { get; set; }

            [Required(ErrorMessage = "Company name is required")]
            [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
            public string CompanyName { get; set; }

            [Url(ErrorMessage = "Website must be a valid URL")]
            public string Website { get; set; }

            [Required(ErrorMessage = "Location is required")]
            [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
            public string Location { get; set; }

            [Url(ErrorMessage = "Profile image must be a valid URL")]
            public string ProfileImageUrl { get; set; }

            [Url(ErrorMessage = "Cover image must be a valid URL")]
            public string CoverImageUrl { get; set; }

            [Url(ErrorMessage = "LinkedIn must be a valid URL")]
            public string LinkedInUrl { get; set; }

            [Url(ErrorMessage = "Facebook must be a valid URL")]
            public string FacebookUrl { get; set; }

            public string PortfolioImages { get; set; }

            public string WorkSamples { get; set; }

            public string Certifications { get; set; }

            [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
            public int? YearsOfExperience { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Total projects completed must be a positive number")]
            public int? TotalProjectsCompleted { get; set; }

            [Range(0, 5, ErrorMessage = "Average rating must be between 0 and 5")]
            public decimal? AverageRating { get; set; }

            public string VerificationDocuments { get; set; }

        }
    }
}
