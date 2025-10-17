using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class OpenAIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _client;

        public OpenAIService(IConfiguration config)
        {
            _apiKey = config["GROQ_API_KEY"];
            if(_apiKey == null)
            {
                throw new Exception("GROQ_API_KEY is not configured in the environment variables.");
            }
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.groq.com/openai/v1/")
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GetAIResponse(string prompt)
        {
            var body = new
            {
                model = "llama-3.1-8b-instant",
                messages = new[]
                {
            new {
                role = "system",
                content =
                    @"Bạn là trợ lý AI gợi ý đối tác hợp tác.
                    Cách trả lời bắt buộc:
                    1️⃣ Trả một câu giới thiệu ngắn gọn (chỉ 1 câu).
                    2️⃣ Sau đó, trả ngay danh sách tối đa 3 đối tác tiềm năng ở định dạng JSON chuẩn, không thêm giải thích, không viết văn bản dư thừa.

                    Ví dụ trả lời:
                    Xin chào! Dưới đây là những đối tác phù hợp:
                    [
                      { 'name': 'Công ty ABC', 'type': 'Nhà tài trợ', 'reason': 'Có kinh nghiệm tổ chức sự kiện tương tự.' },
                      { 'name': 'Nhà cung cấp XYZ', 'type': 'Nhà cung cấp dịch vụ', 'reason': 'Cung cấp thiết bị âm thanh chuyên nghiệp.' }
                    ]

                    Nếu không có đối tác phù hợp, hãy trả:
                    'Xin lỗi, hiện chưa có đối tác nào phù hợp với yêu cầu này.'"
            },
            new { role = "user", content = prompt }
        },
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
