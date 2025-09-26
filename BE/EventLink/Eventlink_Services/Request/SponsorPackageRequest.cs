using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class SponsorPackageRequest
    {
        public class SponsorPackageAddRequest
        {
            [Required]
            public Guid SponsorId { get; set; }

            [Required, StringLength(100, ErrorMessage = "Tên gói không được vượt quá 100 ký tự")]
            public string PackageName { get; set; }

            [Required, StringLength(50, ErrorMessage = "Loại gói không được vượt quá 50 ký tự")]
            public string PackageType { get; set; }

            [Required, Range(1, 100000000, ErrorMessage = "Ngân sách phải lớn hơn 0 và nhỏ hơn 100 triệu")]
            public decimal? Budget { get; set; }

            [StringLength(50)]
            public string BudgetRange { get; set; }

            [Required, StringLength(500, ErrorMessage = "Lợi ích tài trợ không được vượt quá 500 ký tự")]
            public string SponsorshipBenefits { get; set; }

            [Required, StringLength(200, ErrorMessage = "Đối tượng mục tiêu không được vượt quá 200 ký tự")]
            public string TargetAudience { get; set; }

            [Required, StringLength(200, ErrorMessage = "Loại sự kiện ưa thích không được vượt quá 200 ký tự")]
            public string PreferredEventTypes { get; set; }

            [StringLength(300)]
            public string BrandGuidelines { get; set; }

            [Required, Url(ErrorMessage = "LogoUrl phải là đường dẫn hợp lệ")]
            public string LogoUrl { get; set; }

            [StringLength(300, ErrorMessage = "Tài sản thương hiệu không được vượt quá 300 ký tự")]
            public string BrandAssets { get; set; }
        }

        public class SponsorPackageUpdateRequest
        {
            [Required]
            public Guid SponsorId { get; set; }

            [Required, StringLength(100, ErrorMessage = "Tên gói không được vượt quá 100 ký tự")]
            public string PackageName { get; set; }

            [Required, StringLength(50, ErrorMessage = "Loại gói không được vượt quá 50 ký tự")]
            public string PackageType { get; set; }

            [Required, Range(1, 100000000, ErrorMessage = "Ngân sách phải lớn hơn 0 và nhỏ hơn 100 triệu")]
            public decimal? Budget { get; set; }

            [StringLength(50)]
            public string BudgetRange { get; set; }

            [Required, StringLength(500, ErrorMessage = "Lợi ích tài trợ không được vượt quá 500 ký tự")]
            public string SponsorshipBenefits { get; set; }

            [Required, StringLength(200, ErrorMessage = "Đối tượng mục tiêu không được vượt quá 200 ký tự")]
            public string TargetAudience { get; set; }

            [Required, StringLength(200, ErrorMessage = "Loại sự kiện ưa thích không được vượt quá 200 ký tự")]
            public string PreferredEventTypes { get; set; }

            [StringLength(300)]
            public string BrandGuidelines { get; set; }

            [Required, Url(ErrorMessage = "LogoUrl phải là đường dẫn hợp lệ")]
            public string LogoUrl { get; set; }

            [StringLength(300, ErrorMessage = "Tài sản thương hiệu không được vượt quá 300 ký tự")]
            public string BrandAssets { get; set; }
        }
    }
}
