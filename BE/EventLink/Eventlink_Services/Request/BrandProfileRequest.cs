using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class BrandProfileRequest
    {
        public class CreateBrandProfileRequest
        {
            public string BrandName { get; set; }
            public IFormFile BrandLogo { get; set; }
            public string Location { get; set; }
            public string AboutUs { get; set; }
            public string OurMission { get; set; }
            public string Industry { get; set; }
            public string CompanySize { get; set; }
            public string FoundedYear { get; set; }
            public string Website { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Tags { get; set; }
        }

        public class UpdateBrandProfileRequest
        {
            public string BrandName { get; set; }
            public IFormFile BrandLogo { get; set; }
            public string Location { get; set; }
            public string AboutUs { get; set; }
            public string OurMission { get; set; }
            public string Industry { get; set; }
            public string CompanySize { get; set; }
            public string FoundedYear { get; set; }
            public string Website { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Tags { get; set; }
        }
    }
}
