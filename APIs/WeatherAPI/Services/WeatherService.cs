using DeveloprBud.APIs.WeatherAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace DeveloprBud.APIs.WeatherAPI.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherOptions _options;

        public WeatherService(HttpClient httpClient, IOptions<WeatherOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<WeatherResponse?> GetCurrentWeatherAsync(string city)
        {
            var url =
                $"https://api.weatherapi.com/v1/current.json?key={_options.ApiKey}&q={city}";

            var response = await _httpClient.GetAsync(url);

            // return null if the response fails
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<WeatherResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}