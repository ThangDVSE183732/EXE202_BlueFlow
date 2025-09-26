using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Response
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

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

        public bool? IsVerified { get; set; }

        public string VerificationDocuments { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
