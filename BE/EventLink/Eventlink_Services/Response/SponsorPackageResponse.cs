using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Response
{
    public class SponsorPackageResponse
    {
        public Guid SponsorId { get; set; }

        public string PackageName { get; set; }

        public string PackageType { get; set; }

        public decimal? Budget { get; set; }

        public string BudgetRange { get; set; }

        public string SponsorshipBenefits { get; set; }

        public string TargetAudience { get; set; }

        public string PreferredEventTypes { get; set; }

        public string BrandGuidelines { get; set; }

        public string LogoUrl { get; set; }

        public string BrandAssets { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
