using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using Eventlink_Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IEventService _eventService;
        private readonly IBrandProfileService _brandProfileService;
        private readonly IPartnershipService _partnershipService;
        private readonly ILogger<ChatAIController> _logger;

        public ChatAIController(
            OpenAIService openAIService, 
            IUserProfileService userProfileService,
            IEventService eventService,
            IBrandProfileService brandProfileService,
            IPartnershipService partnershipService,
            ILogger<ChatAIController> logger)
        {
            _openAIService = openAIService;
            _userProfileService = userProfileService;
            _eventService = eventService;
            _brandProfileService = brandProfileService;
            _partnershipService = partnershipService;
            _logger = logger;
        }

        [HttpPost("query")]
        public async Task<IActionResult> QueryAI(string message)
        {
            try
            {
                // ✅ Get current user ID and role
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var userRole = GetCurrentUserRole();
                if (string.IsNullOrEmpty(userRole))
                {
                    return Unauthorized(new { success = false, message = "User role not found" });
                }

                object simplifiedData;
                string dataType;
                string aiPrompt;

                // ✅ Lấy dữ liệu theo role
                if (userRole.Equals("Organizer", StringComparison.OrdinalIgnoreCase))
                {
                    // 1️⃣ ORGANIZER: Lấy danh sách Events của user
                    var events = await _eventService.GetEventsByOrganizerIdAsync(userId.Value);
                    
                    // ✅ Check if user has any events
                    if (!events.Any())
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "No events found.",
                            data = new
                            {
                                userRole = userRole,
                                dataType = "Events",
                                results = new object[] { }  // Empty array
                            }
                        });
                    }
                    
                    simplifiedData = events.Select(e => new
                    {
                        e.Id,
                        e.Title,
                        e.Description,
                        e.Location,
                        e.EventDate,
                        e.EventType,
                        e.Category,
                        e.TotalBudget,
                        e.ExpectedAttendees,
                        e.EventHighlights,
                        e.Tags,
                        e.TargetAudienceList
                    });
                    dataType = "Events";
                    
                    // ✅ Ultra-simplified prompt with direct examples
                    var eventsList = events.ToList();
                    var exampleEvent = eventsList.FirstOrDefault();
                    
                    aiPrompt = $@"
You must return ONLY this exact JSON format. Copy the structure exactly:

[
  {{
    ""eventId"": ""{exampleEvent?.Id ?? Guid.NewGuid()}"",
    ""eventTitle"": ""{exampleEvent?.Title ?? "Example Event"}"",
    ""insight"": ""Analyze this event"",
    ""suggestedAction"": ""Provide suggestion""
  }}
]

Here is the event data:
{JsonConvert.SerializeObject(simplifiedData, Formatting.Indented)}

Task: For EACH event in the data, create ONE object following the EXACT format above.

CRITICAL:
- DO NOT nest arrays
- DO NOT add markdown
- Return ONLY the JSON array
- Use the ACTUAL eventId and eventTitle from the data
";
                }
                else if (userRole.Equals("Sponsor", StringComparison.OrdinalIgnoreCase) || 
                         userRole.Equals("Supplier", StringComparison.OrdinalIgnoreCase))
                {
                    // 2️⃣ SPONSOR/SUPPLIER: Lấy BrandProfile + gợi ý Partnerships
                    var brandProfile = await _brandProfileService.GetByUserIdAsync(userId.Value);
                    
                    if (brandProfile == null)
                    {
                        return NotFound(new 
                        { 
                            success = false, 
                            message = "Brand profile not found. Please create your brand profile first." 
                        });
                    }

                    // Lấy tất cả partnerships available (unassigned hoặc public events)
                    var allPartnerships = await _partnershipService.GetUnassignedPartnershipsAsync();
                    
                    simplifiedData = new
                    {
                        brandProfile = new
                        {
                            brandProfile.BrandName,
                            brandProfile.Industry,
                            brandProfile.OurMission,
                            brandProfile.AboutUs,
                            brandProfile.Tags,
                            brandProfile.CompanySize
                        },
                        availablePartnerships = allPartnerships.Select(p => new
                        {
                            p.Id,
                            p.PartnerType,
                            p.ServiceDescription,
                            p.ProposedBudget,
                            p.Status,
                            p.DeadlineDate,
                            Event = p.Event != null ? new
                            {
                                p.Event.Title,
                                p.Event.EventType,
                                p.Event.Location,
                                p.Event.ExpectedAttendees,
                                p.Event.SponsorshipNeeds
                            } : null
                        })
                    };
                    dataType = "BrandProfile + Partnerships";
                    
                    // Prompt riêng cho Sponsor/Supplier
                    aiPrompt = $@"
                        Người dùng yêu cầu: {message}
                        Vai trò: {userRole}
                        
                        Brand Profile của người dùng:
                        {JsonConvert.SerializeObject(((dynamic)simplifiedData).brandProfile, Formatting.Indented)}
                        
                        Danh sách Partnerships available:
                        {JsonConvert.SerializeObject(((dynamic)simplifiedData).availablePartnerships, Formatting.Indented)}

                        Nhiệm vụ:
                        - Phân tích brand profile và tìm TOP 5 partnerships phù hợp nhất
                        - Trả lời CHỈ một JSON array KHÔNG có text thừa
                        - Mỗi phần tử là object với 6 fields
                        
                        Format CHÍNH XÁC (copy exactly):
                        [
                          {{
                            ""partnershipId"": ""actual-guid"",
                            ""eventTitle"": ""event name"",
                            ""partnerType"": ""Sponsor"",
                            ""matchScore"": ""95%"",
                            ""reason"": ""why it matches"",
                            ""suggestedBudget"": ""amount VNĐ""
                          }}
                        ]

                        Ví dụ cụ thể:
                        [
                          {{
                            ""partnershipId"": ""123e4567-e89b-12d3-a456-426614174000"",
                            ""eventTitle"": ""Tech Summit 2024"",
                            ""partnerType"": ""Sponsor"",
                            ""matchScore"": ""95%"",
                            ""reason"": ""Perfect match - Technology industry, 500 attendees, HCMC location"",
                            ""suggestedBudget"": ""200M VNĐ""
                          }},
                          {{
                            ""partnershipId"": ""223e4567-e89b-12d3-a456-426614174001"",
                            ""eventTitle"": ""AI Conference Vietnam"",
                            ""partnerType"": ""Sponsor"",
                            ""matchScore"": ""90%"",
                            ""reason"": ""Strong match - AI focus aligns with your Cloud/IoT tags"",
                            ""suggestedBudget"": ""150M VNĐ""
                          }}
                        ]

                        Tiêu chí matching (ưu tiên giảm dần):
                        1. Industry alignment (40 points)
                        2. Budget compatibility (30 points)
                        3. Location proximity (20 points)
                        4. Event type fit (10 points)

                        QUAN TRỌNG:
                        - KHÔNG thêm markdown ```json
                        - KHÔNG thêm text giải thích
                        - CHỈ trả JSON array thuần
                        - Chọn TỐI ĐA 5 partnerships phù hợp nhất
                        - Nếu không có partnership, trả về: []
                        ";
                }
                else
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Invalid user role. Must be Organizer, Sponsor, or Supplier." 
                    });
                }

                // 3️⃣ Gọi AI service
                var aiResponse = await _openAIService.GetAIResponse(aiPrompt);

                // 4️⃣ Parse JSON từ AI response
                // ✅ Step 1: Clean up response aggressively
                aiResponse = aiResponse?.Trim() ?? string.Empty;
                
                // Remove all markdown variations
                aiResponse = System.Text.RegularExpressions.Regex.Replace(
                    aiResponse, 
                    @"```(json)?", 
                    string.Empty, 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                ).Trim();

                // ✅ Step 2: Extract JSON array with better regex
                var jsonMatch = System.Text.RegularExpressions.Regex.Match(
                    aiResponse, 
                    @"\[\s*\{.*?\}\s*(?:,\s*\{.*?\}\s*)*\]|\[\s*\]",  // Match array of objects OR empty array
                    System.Text.RegularExpressions.RegexOptions.Singleline
                );
                
                var jsonPart = jsonMatch.Success ? jsonMatch.Value : string.Empty;

                object parsedData;
                
                if (string.IsNullOrWhiteSpace(jsonPart))
                {
                    // ✅ FALLBACK: If AI fails, create structure manually
                    _logger.LogWarning($"[QueryAI] No valid JSON found. Creating fallback response.");
                    
                    if (userRole.Equals("Organizer", StringComparison.OrdinalIgnoreCase))
                    {
                        // Create response from events data directly
                        var events = await _eventService.GetEventsByOrganizerIdAsync(userId.Value);
                        parsedData = events.Select(e => new
                        {
                            eventId = e.Id.ToString(),
                            eventTitle = e.Title,
                            insight = $"Event scheduled for {e.EventDate:yyyy-MM-dd} in {e.Location}",
                            suggestedAction = $"Review budget ({e.TotalBudget:N0} VNĐ) and sponsorship needs"
                        }).ToArray();
                    }
                    else
                    {
                        parsedData = new object[] { };
                    }
                }
                else
                {
                    try
                    {
                        // ✅ Step 3: Parse JSON
                        var tempData = JsonConvert.DeserializeObject(jsonPart);
                        
                        // ✅ Step 4: Aggressive filtering
                        if (tempData is Newtonsoft.Json.Linq.JArray array)
                        {
                            var cleanArray = new Newtonsoft.Json.Linq.JArray();
                            
                            foreach (var item in array)
                            {
                                // ✅ Recursive function to find actual objects
                                var actualObject = FindActualObject(item);
                                if (actualObject != null)
                                {
                                    cleanArray.Add(actualObject);
                                }
                            }
                            
                            // ✅ FALLBACK: If still empty after filtering, create manually
                            if (cleanArray.Count == 0 && userRole.Equals("Organizer", StringComparison.OrdinalIgnoreCase))
                            {
                                _logger.LogWarning("[QueryAI] Parsed array is empty. Creating manual fallback.");
                                var events = await _eventService.GetEventsByOrganizerIdAsync(userId.Value);
                                parsedData = events.Select(e => new
                                {
                                    eventId = e.Id.ToString(),
                                    eventTitle = e.Title,
                                    insight = $"Event: {e.EventType} - {e.Category}",
                                    suggestedAction = $"Focus on attracting sponsors for {e.ExpectedAttendees} attendees"
                                }).ToArray();
                            }
                            else
                            {
                                parsedData = cleanArray;
                            }
                            
                            _logger.LogInformation($"[QueryAI] Parsed {(parsedData is Array arr ? arr.Length : 0)} results from AI response");
                        }
                        else
                        {
                            parsedData = new object[] { };
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[QueryAI] Failed to parse AI response as JSON");
                        
                        // ✅ Fallback: Create manual response
                        if (userRole.Equals("Organizer", StringComparison.OrdinalIgnoreCase))
                        {
                            var events = await _eventService.GetEventsByOrganizerIdAsync(userId.Value);
                            parsedData = events.Select(e => new
                            {
                                eventId = e.Id.ToString(),
                                eventTitle = e.Title,
                                insight = $"Budget: {e.TotalBudget:N0} VNĐ, Attendees: {e.ExpectedAttendees}",
                                suggestedAction = "Review and optimize event plan"
                            }).ToArray();
                        }
                        else
                        {
                            parsedData = new object[] { };
                        }
                    }
                }

                // 5️⃣ Trả kết quả JSON array
                return Ok(new
                {
                    success = true,
                    message = "AI query successful.",
                    data = new
                    {
                        userRole = userRole,
                        dataType = dataType,
                        results = parsedData  // ✅ JSON array thay vì text
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[QueryAI] Exception occurred");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Internal server error", 
                    error = ex.Message 
                });
            }
        }

        // ✅ Helper method to recursively find actual JObject in nested arrays
        private Newtonsoft.Json.Linq.JObject FindActualObject(Newtonsoft.Json.Linq.JToken token)
        {
            if (token is Newtonsoft.Json.Linq.JObject obj)
            {
                // Found an object - check if it has properties
                return obj.HasValues ? obj : null;
            }
            
            if (token is Newtonsoft.Json.Linq.JArray arr)
            {
                // Search recursively in nested arrays
                foreach (var item in arr)
                {
                    var found = FindActualObject(item);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            
            return null;
        }

        // Helper method to get current user ID
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
        }

        // Helper method to get current user role
        private string GetCurrentUserRole()
        {
            return User.FindFirst("Role")?.Value;
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