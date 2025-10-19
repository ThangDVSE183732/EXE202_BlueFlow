using Eventlink_Services.Interface;
using Eventlink_Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Eventlink_Services.Request.PartnershipRequest;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PartnershipsController : ControllerBase
    {
        private readonly IPartnershipService _partnershipService;
        private readonly IUserProfileService _userProfileService;
        private readonly OpenAIService _openAIService;

        public PartnershipsController(IPartnershipService partnershipService, OpenAIService openAIService, IUserProfileService userProfileService)
        {
            _partnershipService = partnershipService;
            _openAIService = openAIService;
            _userProfileService = userProfileService;
        }

        [HttpPost]
        //[Authorize(Roles = "Organizer")]
        public async Task<IActionResult> CreatePartnership([FromBody] CreatePartnershipRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("UserId").Value);

            if (userId == null)
            {
                return Unauthorized();
            }
            
            var result = await _partnershipService.CreateAsync(userId, request);

            return Ok(new
            {
                success = true,
                message = "Partnership request sent successfully.",
                data = result
            });
        }

        [HttpPut("{id}/status")]
        //[Authorize(Roles = "Partner")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePartnershipStatusRequest request)
        {
            var result = await _partnershipService.UpdateStatusAsync(id, request.Status, request.OrganizerResponse);

            return Ok(new
            {
                success = true,
                message = $"Partnership {request.Status.ToLower()} successfully.",
                data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartnership(Guid id, [FromBody] UpdatePartnershipRequest request)
        {
            await _partnershipService.UpdateAsync(id, request);

            return Ok(new
            {
                success = true,
                message = "Partnership updated successfully.",
            });
        }

        [HttpPost("query")]
        public async Task<IActionResult> QueryAI(string message)
        {
            // 1️⃣ Lấy dữ liệu hồ sơ người dùng
            var userProfiles = await _userProfileService.GetAllUserProfilesAsync();

            // 2️⃣ Chỉ chọn các trường cần thiết để AI phân tích
            var simplifiedProfiles = userProfiles.Select(p => new
            {
                p.Id,
                p.UserId,
                p.FullName,
                p.Email,
                p.Role,
                p.CompanyName,
                p.Bio,
                p.AverageRating,
                p.YearsOfExperience
            });

            // 3️⃣ Tạo prompt cho AI
            var prompt = $@"
                Người dùng yêu cầu: {message}

                Dưới đây là danh sách các đối tác trong hệ thống:
                {JsonConvert.SerializeObject(simplifiedProfiles, Formatting.Indented)}

                Nhiệm vụ:
                - Phân tích dữ liệu và chọn tối đa 3 đối tác phù hợp nhất với yêu cầu người dùng.
                - Trả lời đúng định dạng sau:
                1️⃣ Một câu giới thiệu ngắn gọn.
                2️⃣ Ngay sau đó là danh sách JSON gồm tối đa 3 đối tác:
                [
                  {{
                    'id': '', 
                    'name': '', 
                    'role': '', 
                    'companyName': '', 
                    'bio': '', 
                    'averageRating': '', 
                    'yearsOfExperience': ''
                  }}
                ]

                Không thêm mô tả, không giải thích thừa.  
                Nếu không có ai phù hợp, hãy trả:
                'Xin lỗi, hiện chưa có đối tác nào phù hợp với yêu cầu này.'
                ";

            // 4️⃣ Gọi AI service
            var aiResponse = await _openAIService.GetAIResponse(prompt);

            // 5️⃣ Tách phần JSON trong câu trả lời
            var jsonPart = System.Text.RegularExpressions.Regex
                .Match(aiResponse, @"\[.*\]", System.Text.RegularExpressions.RegexOptions.Singleline)
                .Value;

            object parsedJson = null;
            try
            {
                parsedJson = JsonConvert.DeserializeObject(jsonPart);
            }
            catch
            {
                parsedJson = aiResponse;
            }

            // 6️⃣ Trả kết quả cho frontend
            return Ok(new
            {
                success = true,
                message = "AI query successful.",
                data = parsedJson
            });
        }
    }
}
