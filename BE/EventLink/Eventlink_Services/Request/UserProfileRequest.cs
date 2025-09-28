using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class UserProfileRequest
    {
        public class CreateUserProfileRequest
        {
            public string Bio { get; set; }

            public string CompanyName { get; set; }

            public string Website { get; set; }

            public string Location { get; set; }

            public string ProfileImageUrl { get; set; }

            public string CoverImageUrl { get; set; }

            public string LinkedInUrl { get; set; }

            public string FacebookUrl { get; set; }

            public string PortfolioImages { get; set; }

            public string WorkSamples { get; set; }

            public string Certifications { get; set; }

            public int? YearsOfExperience { get; set; }

            public int? TotalProjectsCompleted { get; set; }

            public decimal? AverageRating { get; set; }

            public string VerificationDocuments { get; set; }
        }

        public class UpdateUserProfileRequest
        {
            public string Bio { get; set; }

            public string CompanyName { get; set; }

            public string Website { get; set; }

            public string Location { get; set; }

            public string ProfileImageUrl { get; set; }

            public string CoverImageUrl { get; set; }

            public string LinkedInUrl { get; set; }

            public string FacebookUrl { get; set; }

            public string PortfolioImages { get; set; }

            public string WorkSamples { get; set; }

            public string Certifications { get; set; }

            public int? YearsOfExperience { get; set; }

            public int? TotalProjectsCompleted { get; set; }

            public decimal? AverageRating { get; set; }

            public string VerificationDocuments { get; set; }

        }
    }
}
