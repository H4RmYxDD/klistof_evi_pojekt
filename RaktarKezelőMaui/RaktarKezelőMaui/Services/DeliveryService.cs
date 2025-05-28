using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;

namespace RaktarKezeloMaui.Services
{
    public class DeliveryService
    {
        private readonly HttpClient _httpClient;

        public DeliveryService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://your-api-base-url.com/api/delivery/")
            };
        }

        public async Task<PackageResponse> SendPackageAsync(Package package)
        {
            var response = await _httpClient.PostAsJsonAsync("send", package);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PackageResponse>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception(error?.Error ?? "Ismeretlen hiba történt.");
            }
        }

        public async Task<StatusResponse> GetStatusAsync(Guid packageId)
        {
            var response = await _httpClient.GetAsync($"status/{packageId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<StatusResponse>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception(error?.Error ?? "Ismeretlen hiba történt.");
            }
        }
    }

    public class Package
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
    }

    public class PackageResponse
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
    }

    public class StatusResponse
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
    }
}
