using Eventlink_Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class PayOSHttpService : IPayOSService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;
        private readonly string _defaultReturnUrl;
        private readonly string _defaultCancelUrl;
        private const string PAYOS_BASE_URL = "https://api-merchant.payos.vn";

        public PayOSHttpService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;

            _clientId = _config["PayOSSettings:ClientId"] ?? Environment.GetEnvironmentVariable("PAYOS_CLIENT_ID");
            _apiKey = _config["PayOSSettings:ApiKey"] ?? Environment.GetEnvironmentVariable("PAYOS_API_KEY");
            _checksumKey = _config["PayOSSettings:ChecksumKey"] ?? Environment.GetEnvironmentVariable("PAYOS_CHECKSUM_KEY");

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_checksumKey))
            {
                throw new ArgumentException("PayOS credentials are not configured properly");
            }

            _defaultReturnUrl = _config["PayOSSettings:ReturnUrl"] ?? Environment.GetEnvironmentVariable("PAYOS_RETURN_URL");
            _defaultCancelUrl = _config["PayOSSettings:CancelUrl"] ?? Environment.GetEnvironmentVariable("PAYOS_CANCEL_URL");

            Console.WriteLine("[PayOSHttp] Service initialized");
        }

        public async Task<dynamic> CreatePaymentLinkAsync(
             long orderCode,
             long amount,
             string description,
             string buyerName,
             string buyerEmail,
             string returnUrl = null,
             string cancelUrl = null)
        {
            try
            {
                var actualReturnUrl = returnUrl ?? _defaultReturnUrl;
                var actualCancelUrl = cancelUrl ?? _defaultCancelUrl;

                var payload = new
                {
                    orderCode = orderCode,
                    amount = amount,
                    description = description?.Length > 25 ? description.Substring(0, 25) : "Premium",
                    items = new[]
                    {
                new
                {
                    name = "Premium",
                    quantity = 1,
                    price = amount
                }
            },
                    returnUrl = actualReturnUrl,
                    cancelUrl = actualCancelUrl,
                    buyerName = buyerName,
                    buyerEmail = buyerEmail
                };

                var json = JsonSerializer.Serialize(payload);
                Console.WriteLine($"[PayOSHttp] Request: {json}");

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(PAYOS_BASE_URL);
                httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
                httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/v2/payment-requests", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[PayOSHttp] Status: {response.StatusCode}");
                Console.WriteLine($"[PayOSHttp] Response: {responseContent}");

                // ✅ Parse response
                var result = JsonDocument.Parse(responseContent);
                var code = result.RootElement.GetProperty("code").GetString();
                var desc = result.RootElement.GetProperty("desc").GetString();

                // ✅ Check if success
                if (code != "00")
                {
                    throw new Exception($"PayOS Error {code}: {desc}");
                }

                // ✅ Check if data exists
                if (!result.RootElement.TryGetProperty("data", out var dataElement) || dataElement.ValueKind == JsonValueKind.Null)
                {
                    throw new Exception($"PayOS returned no data: {desc}");
                }

                return dataElement;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PayOSHttp] Error: {ex.Message}");
                throw new Exception($"PayOS HTTP request failed: {ex.Message}", ex);
            }
        }

        public async Task<dynamic> GetPaymentInfoAsync(string paymentLinkId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(PAYOS_BASE_URL);
            httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
            httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            var response = await httpClient.GetAsync($"/v2/payment-requests/{paymentLinkId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"PayOS API error: {responseContent}");
            }

            var result = JsonDocument.Parse(responseContent);
            return result.RootElement.GetProperty("data");
        }

        public async Task<dynamic> CancelPaymentLinkAsync(string paymentLinkId, string cancellationReason = null)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(PAYOS_BASE_URL);
            httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
            httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            var payload = new { cancellationReason = cancellationReason ?? "Cancelled by user" };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/v2/payment-requests/{paymentLinkId}/cancel", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"PayOS API error: {responseContent}");
            }

            var result = JsonDocument.Parse(responseContent);
            return result.RootElement.GetProperty("data");
        }

        public bool VerifyWebhookData(string webhookBody)
        {
            try
            {
                if (string.IsNullOrEmpty(webhookBody))
                    return false;

                var test = JsonDocument.Parse(webhookBody);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public long GenerateOrderCode()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}