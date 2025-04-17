﻿using Microsoft.Extensions.Logging;
using RemindMe.Models.ResponseModels;
using RemindMe.Services;
using System.Text;
using System.Text.Json;

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
                _logger.LogInformation($"Creating room {roomName}");

                var deviceId = GetDeviceId();

                var request = new HttpRequestMessage(HttpMethod.Post, $"{SystemConstants.BACKEND_URL}/room/createRoom?deviceId={deviceId}");
                request.Content = new StringContent(JsonSerializer.Serialize(new { 
                    RoomName = roomName, Password = password
                }), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                return new BackendClientResponseResult<Guid>()
                {
                    Success = response.IsSuccessStatusCode,
                    Data = Guid.Parse(response.Content.ReadAsStringAsync().Result.Replace("\"", ""))
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Unhandled exception while creating a room: {e.Message}");
                return new BackendClientResponseResult<Guid>()
                {
                    Success = false,
                    Message = e.Message,
                    Data = Guid.Empty
                };
            }
        }

        public async Task<BackendClientResponseResult<string>> JoinRoom(Guid userId, string roomName, string password)
        {
            try
            {
                _logger.LogInformation($"Creating room {roomName}");

                var deviceId = GetDeviceId();

                var request = new HttpRequestMessage(HttpMethod.Post, $"{SystemConstants.BACKEND_URL}/room/joinRoom?deviceId={deviceId}");
                request.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    RoomName = roomName,
                    Password = password
                }), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                return new BackendClientResponseResult<string>()
                {
                    Success = response.IsSuccessStatusCode,
                    Data = response.Content.ReadAsStringAsync().Result.Replace("\"", "")
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Unhandled exception while creating a room: {e.Message}");
                return new BackendClientResponseResult<string>()
                {
                    Success = false,
                    Message = e.Message,
                    Data = ""
                };
            }
        }

        public async Task<BackendClientResponseResult<string>> SwapRole()
        {
            try
            {
                var deviceId = GetDeviceId();

                _logger.LogInformation($"Swapping role for user {deviceId}");

                var request = new HttpRequestMessage(HttpMethod.Put, $"{SystemConstants.BACKEND_URL}/room/swapRole?deviceId={deviceId}");

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                return new BackendClientResponseResult<string>()
                {
                    Success = response.IsSuccessStatusCode,
                    Data = response.Content.ReadAsStringAsync().Result.Replace("\"", "")
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Unhandled exception while swapping user role: {e.Message}");
                return new BackendClientResponseResult<string>()
                {
                    Success = false,
                    Message = e.Message,
                    Data = ""
                };
            }
        }

        public async Task<BackendClientResponseResult<string>> PublishVideo(FileResult file, DateTime date, TimeSpan time)
        {
            try
            {
                _logger.LogInformation($"Uploading video: {file?.FileName}");

                var deviceId = GetDeviceId(); 
                var uploadUrl = $"{SystemConstants.BACKEND_URL}/video/upload";

                using var videoStream = await file.OpenReadAsync();
                using var streamContent = new StreamContent(videoStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                var content = new MultipartFormDataContent();
                content.Add(streamContent, "video", file.FileName);
                content.Add(new StringContent(deviceId), "deviceId");
                content.Add(new StringContent(date.ToString("yyyy-MM-dd")), "date");
                content.Add(new StringContent(time.ToString(@"hh\:mm")), "time");

                var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync();

                return new BackendClientResponseResult<string>
                {
                    Success = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? "Video uploaded successfully." : responseBody,
                    Data = response.IsSuccessStatusCode ? responseBody : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unhandled exception while uploading video: {ex.Message}");
                return new BackendClientResponseResult<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<BackendClientResponseResult<string>> GetLatestVideo()
        {
            try
            {
                var deviceId = GetDeviceId();

                var fileName = "local_copy.mp4";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // Don't take up too much space on mobile
                if (File.Exists(filePath))
                    File.Delete(filePath);

                _logger.LogInformation($"Getting latest video for device {deviceId}");
                var uploadUrl = $"{SystemConstants.BACKEND_URL}/video/latest?deviceId={deviceId}";

                var request = new HttpRequestMessage(HttpMethod.Get, uploadUrl);

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    return new BackendClientResponseResult<string>
                    {
                        Success = false,
                        Message = errorText,
                        Data = null
                    };
                }

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = File.Create(filePath))
                {
                    await stream.CopyToAsync(fileStream);
                }

                return new BackendClientResponseResult<string>
                {
                    Success = true,
                    Message = "Video downloaded successfully.",
                    Data = filePath // return local path to the video
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unhandled exception while downloading video: {ex.Message}");
                return new BackendClientResponseResult<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
