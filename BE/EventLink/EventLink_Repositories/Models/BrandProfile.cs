using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Models
{
    public partial class BrandProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string BrandName { get; set; }
        public string BrandLogo { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual User User { get; set; }
    }
}
