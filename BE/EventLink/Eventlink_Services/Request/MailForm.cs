using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class MailForm
    {
        public class MailRequest
        {
            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }

        public class VerifyOtpRequest
        {
            [Required]
            public string Email { get; set; }
            [Required, StringLength(6, MinimumLength = 6, ErrorMessage = "OTP phải có 6 ký tự")]
            public string Otp { get; set; }
        }

        public class VerifyRegisterOtpWithProfileRequest
        {
            public VerifyOtpRequest OtpRequest { get; set; }
            public CreateUserProfileRequest ProfileRequest { get; set; }
        }
    }
}
