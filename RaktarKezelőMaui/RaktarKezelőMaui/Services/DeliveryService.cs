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
                BaseAddress = new Uri("http://127.0.0.1:5240")
            };
        }

        public async Task<PackageResponse> SendPackageAsync(Package package)
        {
            var response = await _httpClient.PostAsJsonAsync("api/delivery/receive", package);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PackageResponse>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception(error?.error ?? "Ismeretlen hiba történt.");
            }
        }

        public async Task<string> GetStatusAsync(Guid packageId)
        {
            var response = await _httpClient.GetAsync($"api/delivery/status/{packageId}");
            if (response.IsSuccessStatusCode)
            {
                // The API returns a plain string, not a JSON object
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception(error?.error ?? "Ismeretlen hiba történt.");
            }
        }
        public async Task<List<Package>> GetPackagesAsync()
        {
            var response = await _httpClient.GetAsync("api/delivery/packages");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Package>>();
            throw new Exception("Failed to fetch packages.");
        }

        public async Task DeletePackageAsync(Guid packageId)
        {
            var response = await _httpClient.DeleteAsync($"api/delivery/{packageId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to delete package.");
        }
    }

    public class Package
    {
        public Guid Id { get; set; }
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

    public class ErrorResponse
    {
        public string error { get; set; }
    }
}