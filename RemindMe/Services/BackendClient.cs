using Android.Content;
using Java.Util;
using Microsoft.Extensions.Logging;
using RemindMe.Models.ResponseModels;
using RemindMe.Services;
using System.Text;
using System.Text.Json;
using static Android.Provider.Settings;

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

        private string GetDeviceId()
        {
            string deviceID = "";

#if ANDROID

            deviceID = Android.Provider.Settings.Secure.GetString(Platform.CurrentActivity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

#elif IOS
                deviceID = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
#endif

            return deviceID;
        }

        public async Task<BackendClientResponseResult<Guid>> GenerateUserId()
        {
            try
            {
                _logger.LogInformation("Generating a new User ID");

                var deviceId = GetDeviceId();

                var request = new HttpRequestMessage(HttpMethod.Get, $"{SystemConstants.BACKEND_URL}/User?deviceId={deviceId}");

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

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

        public async Task<BackendClientResponseResult<Guid>> CreateRoom(Guid userId, string roomName, string password)
        {
            try
            {
                _logger.LogInformation("Generating a new User ID");

                var deviceId = GetDeviceId();

                var request = new HttpRequestMessage(HttpMethod.Post, $"{SystemConstants.BACKEND_URL}/room/createRoom?deviceId={deviceId}");
                request.Content = new StringContent(JsonSerializer.Serialize(new { 
                    RoomName = roomName, Password = password
                }), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

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
