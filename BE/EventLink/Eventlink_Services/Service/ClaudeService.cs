using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class ClaudeService
    {
        private readonly string _apiKey;
        private readonly HttpClient _client;
        private const string CLAUDE_API_URL = "https://api.anthropic.com/v1/messages";

        public ClaudeService(IConfiguration config)
        {
            _apiKey = Environment.GetEnvironmentVariable("CLAUDE_API_KEY") 
                     ?? config["CLAUDE_API_KEY"];
            
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("CLAUDE_API_KEY is not configured in environment variables or configuration.");
            }

            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(120); // 120 seconds timeout for Claude API
            _client.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Call Claude Haiku API to format response
        /// </summary>
        public async Task<string> FormatResponseAsync(string systemPrompt, string userPrompt)
        {
            try
            {
                var requestBody = new
                {
                    model = "claude-3-5-haiku-20241022",
                    max_tokens = 4096,
                    system = systemPrompt,
                    messages = new[]
                    {
                        new { role = "user", content = userPrompt }
                    }
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _client.PostAsync(CLAUDE_API_URL, content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Claude API error: {response.StatusCode} - {json}");
                }

                dynamic parsed = JsonConvert.DeserializeObject(json);
                string text = parsed?.content?[0]?.text?.ToString();
                
                return text ?? json;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calling Claude API: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Simple method to get AI response
        /// </summary>
        public async Task<string> GetResponseAsync(string prompt)
        {
            var systemPrompt = "Bạn là trợ lý AI chuyên format response tiếng Việt cho hệ thống partnership matching.";
            return await FormatResponseAsync(systemPrompt, prompt);
        }
    }
}

