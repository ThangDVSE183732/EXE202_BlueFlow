using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class GoogleAuthRequest
    {
        [Required(ErrorMessage = "Google ID token is required")]
        public string IdToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Organizer|Supplier|Sponsor)$", ErrorMessage = "Role must be Organizer, Supplier, or Sponsor")]
        public string Role { get; set; } = string.Empty;
    }

    public class GoogleUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string GoogleId { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
    }
}
