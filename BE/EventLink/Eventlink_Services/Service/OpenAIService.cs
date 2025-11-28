using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Eventlink_Services.Service
{
    public class OpenAIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _client;
        private readonly string _systemKnowledge;

        public OpenAIService(IConfiguration config)
        {
            _apiKey = config["GROQ_API_KEY"];
            if (_apiKey == null)
            {
                throw new Exception("GROQ_API_KEY is not configured in the environment variables.");
            }
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.groq.com/openai/v1/")
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Load your system knowledge
            _systemKnowledge = LoadSystemKnowledge();
        }

        private string LoadSystemKnowledge()
        {
            try
            {
                // Get the base directory of the application
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var knowledgePath = Path.Combine(basePath, "Knowledge", "System_docs.txt");

                // Check if file exists
                if (!File.Exists(knowledgePath))
                {
                    throw new FileNotFoundException($"System knowledge file not found at: {knowledgePath}");
                }

                // Read the file content
                return File.ReadAllText(knowledgePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading system knowledge: {ex.Message}");
            }
        }

        public async Task<string> GetAIResponse(string prompt, bool includeSystemKnowledge = false)
        {
            var messages = new List<object>();

            // System prompt rút gọn
            var systemContent = @"Bạn là trợ lý AI gợi ý đối tác hợp tác.
    Bắt buộc:
    - Trả về đúng định dạng: một câu giới thiệu (nếu có) + JSON chuẩn.
    - Không dùng markdown, không giải thích thêm.";

            if (includeSystemKnowledge)
                systemContent += "\n\n" + _systemKnowledge;

            messages.Add(new { role = "system", content = systemContent });
            messages.Add(new { role = "user", content = prompt });

            var body = new
            {
                model = "llama-3.1-8b-instant",
                messages,
                temperature = 0.4
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("chat/completions", content);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var json = Encoding.UTF8.GetString(bytes);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Groq API error: {response.StatusCode} - {json}");

            dynamic parsed = JsonConvert.DeserializeObject(json);
            string text = parsed?.choices?[0]?.message?.content?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(text))
                return json;

            var match = System.Text.RegularExpressions.Regex.Match(text, @"```json\s*(\[.*?\])\s*```", System.Text.RegularExpressions.RegexOptions.Singleline);
            if (match.Success)
                text = match.Groups[1].Value;
            else
            {
                var altMatch = System.Text.RegularExpressions.Regex.Match(text, @"(\[.*?\])", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (altMatch.Success)
                    text = altMatch.Groups[1].Value;
            }

            return text;
        }

        // New method for general questions about the system
        public async Task<string> AskAboutSystem(string question)
        {
            var systemPrompt = $@"Bạn là trợ lý AI của hệ thống Eventlink.
Hãy trả lời câu hỏi của người dùng dựa trên thông tin hệ thống dưới đây.
Trả lời ngắn gọn, rõ ràng bằng tiếng Việt.

{_systemKnowledge}";

            var body = new
            {
                model = "llama-3.1-8b-instant",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = question }
                },
                temperature = 0.3
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync("chat/completions", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode} - {json}";
            }

            dynamic parsed = JsonConvert.DeserializeObject(json);
            string text = parsed?.choices?[0]?.message?.content?.ToString();
            return text ?? json;
        }

        // Method with conversation history support
        public async Task<string> GetAIResponseWithHistory(string prompt, List<(string role, string content)> conversationHistory = null)
        {
            var messages = new List<object>();

            // Add system message
            messages.Add(new
            {
                role = "system",
                content = $@"Bạn là trợ lý AI của hệ thống Eventlink.

{_systemKnowledge}

Trả lời các câu hỏi của người dùng dựa trên thông tin hệ thống và lịch sử hội thoại."
            });

            // Add conversation history if provided
            if (conversationHistory != null)
            {
                foreach (var msg in conversationHistory)
                {
                    messages.Add(new { role = msg.role, content = msg.content });
                }
            }

            // Add current prompt
            messages.Add(new { role = "user", content = prompt });

            var body = new
            {
                model = "llama-3.1-8b-instant",
                messages = messages,
                temperature = 0.5
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync("chat/completions", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode} - {json}";
            }

            dynamic parsed = JsonConvert.DeserializeObject(json);
            string text = parsed?.choices?[0]?.message?.content?.ToString();
            return text ?? json;
        }
    }
}