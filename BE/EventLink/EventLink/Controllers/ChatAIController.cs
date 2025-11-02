using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using Eventlink_Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var userProfiles = await _userProfileService.GetAllUserProfilesAsync();
            var simplifiedProfiles = userProfiles.Select(p => new {
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
                p.Role
            });

            var prompt = $@"
Người dùng yêu cầu: {message}

Danh sách đối tác:
{JsonConvert.SerializeObject(simplifiedProfiles, Formatting.Indented)}

Trả về ĐÚNG định dạng JSON array (không thêm markdown, không giải thích):
[
  {{
    ""name"": ""Tên người"",
    ""role"": ""Vai trò"",
    ""companyName"": ""Tên công ty"",
    ""industry"": ""Ngành nghề"",
    ""country"": ""Quốc gia"",
    ""city"": ""Thành phố"",
    ""jobTitle"": ""Chức vụ"",
    ""directEmail"": ""email@example.com"",
    ""directPhone"": ""+84 xxx xxx xxx""
  }}
]

Nếu không có kết quả, trả: []
";

            var aiResponse = await _openAIService.GetAIResponse(prompt);

            Console.WriteLine("========= RAW AI RESPONSE =========");
            Console.WriteLine(aiResponse);
            Console.WriteLine("========= END RAW RESPONSE =========");

            // Xử lý response
            string jsonString = aiResponse.Trim();

            // Loại bỏ markdown nếu có
            var markdownMatch = Regex.Match(jsonString, @"```(?:json)?\s*([\s\S]*?)\s*```", RegexOptions.Singleline);
            if (markdownMatch.Success)
            {
                jsonString = markdownMatch.Groups[1].Value.Trim();
            }

            // Tìm JSON array trong text
            var jsonMatch = Regex.Match(jsonString, @"(\[[\s\S]*?\])", RegexOptions.Singleline);
            if (jsonMatch.Success)
            {
                jsonString = jsonMatch.Groups[1].Value.Trim();
            }

            Console.WriteLine("========= EXTRACTED JSON =========");
            Console.WriteLine(jsonString);
            Console.WriteLine("========= END EXTRACTED JSON =========");

            // Parse JSON CHỈ MỘT LẦN
            try
            {
                // Validate JSON trước
                var testParse = JToken.Parse(jsonString); // Dùng JToken để validate

                if (testParse.Type == JTokenType.Array)
                {
                    var resultArray = testParse.ToObject<List<Dictionary<string, object>>>();

                    if (resultArray == null || resultArray.Count == 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Không tìm thấy đối tác phù hợp.",
                            data = new List<object>()
                        });
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "AI query successful.",
                        data = resultArray
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Response không đúng định dạng array.",
                        data = new { text = jsonString }
                    });
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ JSON Parse Error: {ex.Message}");

                return Ok(new
                {
                    success = false,
                    message = "Không thể parse JSON response.",
                    data = new
                    {
                        error = ex.Message,
                        rawText = jsonString
                    }
                });
            }
        }

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