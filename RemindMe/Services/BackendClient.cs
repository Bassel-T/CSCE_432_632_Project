using Microsoft.Extensions.Logging;
using RemindMe.Models.ResponseModels;
using RemindMe.Services;

namespace RemindMe.Services
{
    public class BackendClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public BackendClient(ILogger<BackendClient> logger)
        {
            _logger = logger;
   
            _httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        public async Task<BackendClientResponseResult<Guid>> GenerateUserId()
        {
            try
            {
                _logger.LogInformation("Generating a new User ID");

                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"{SystemConstants.BACKEND_URL}/User")).ConfigureAwait(false);

                return new BackendClientResponseResult<Guid>()
                {
                    Success = response.IsSuccessStatusCode,
                    // If it can't generate a user, it'll throw an error anyway
                    Data = Guid.Parse(response.Content.ReadAsStringAsync().Result.Replace("\"", ""))
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Unhandled exception while generating a User ID: {e.Message}");
                return new BackendClientResponseResult<Guid>()
                {
                    Success = false,
                    Message = e.Message,
                    Data = Guid.Empty
                };
            }
        }
    }
}
