using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using Eventlink_Services.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatAIController : ControllerBase
    {
        private readonly OpenAIService _openAIService;
        private readonly IUserProfileService _userProfileService;
        public ChatAIController(OpenAIService openAIService, IUserProfileService userProfileService)
        {
            _openAIService = openAIService;
            _userProfileService = userProfileService;
        }

        [HttpPost("query")]
        public async Task<IActionResult> QueryAI(string message)
        {
            // 1️ Lấy dữ liệu hồ sơ người dùng
            var userProfiles = await _userProfileService.GetAllUserProfilesAsync();

            // 2️ Chỉ chọn các trường cần thiết để AI phân tích
            var simplifiedProfiles = userProfiles.Select(p => new
            {
                p.CompanyName,
                p.Industry,
                p.CompanySize,
                p.FoundedYear,
                p.City,
                p.CountryRegion,
                p.FullName,
                p.JobTitle,
                p.DirectEmail,
                p.DirectPhone,
                p.Role,
            });

            // 3️ Tạo prompt cho AI
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
                    'name': '', 
                    'role': '', 
                    'companyName': '', 
                    'industry': '', 
                    'country': '', 
                    'city': '', 
                    'jobTitle': '', 
                    'directEmail': '', 
                    'directPhone': '',
                    'role': ''    
                  }}
                ]

                Không thêm mô tả, không giải thích thừa.  
                Nếu không có ai phù hợp, hãy trả:
                'Xin lỗi, hiện chưa có đối tác nào phù hợp với yêu cầu này.'
                ";

            // 4️ Gọi AI service
            var aiResponse = await _openAIService.GetAIResponse(prompt);

            // 5️ Tách phần JSON trong câu trả lời
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

            // 6️ Trả kết quả cho frontend
            return Ok(new
            {
                success = true,
                message = "AI query successful.",
                data = parsedJson
            });
        }

        // New endpoint for system questions
        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] QuestionRequest request)
        {
            var response = await _openAIService.AskAboutSystem(request.Question);
            return Ok(new { answer = response });
        }

        // Endpoint with conversation history
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var response = await _openAIService.GetAIResponseWithHistory(
                request.Message,
                request.History
            );
            return Ok(new { answer = response });
        }
    }

    public class QuestionRequest
    {
        public string Question { get; set; }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
        public List<(string role, string content)> History { get; set; }
    }
}