using System.Diagnostics;
using System.Text;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting;
using PetProject.OrderManagement.Domain.DTOs.User.Request;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Repositories;
using PetProject.OrderManagement.Domain.Services;
using PetProject.OrderManagement.Persistence.Services;

namespace PetProject.OrderManagement.Application.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<UserService> _logger;

        private readonly AppSettings _appSettings;

        private Stopwatch _stopwatch;

        public UserService(
            IUserRepository userRepository,
            IDateTimeProvider dateTimeProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserService> logger,
            AppSettings appSettings) 
        { 
            _userRepository = userRepository;
            _dateTimeProvider = dateTimeProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task CreateUser(UserDto userInformation)
        {
            _stopwatch = Stopwatch.StartNew();
            var ipAddress = GetIpAddress();

            try
            {
                var user = _userRepository.GetAll().Where(x => x.Id == userInformation.Id).FirstOrDefault();

                if (user == null)
                {
                    await _userRepository.AddAsync(Mapper.ToUser(userInformation));

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_appSettings.InternalAPI.BaseAPI);
                        var uri = $"{_appSettings.InternalAPI.BaseAPI}/{_appSettings.InternalAPI.UserAPI.Register}";
                        var content = new StringContent(JsonConvert.SerializeObject(userInformation), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(uri, content);

                        if (response == null)
                        {
                            LogTrace(userInformation.UserName, "", ipAddress, $"[UserService - RegisterNewUser] Internal Error");
                            throw new Exception("Internal Error");
                        }
                        else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            LogTrace(userInformation.UserName, "", ipAddress, $"[UserService - RegisterNewUser] Internal Error with status code response: {(int)response.StatusCode}");
                            throw new HttpRequestException($"Internal Error with status code response: {(int)response.StatusCode}");
                        }

                        await _userRepository.SaveChangesAsync();
                    }
                }
                else
                {
                    LogTrace("", "", ipAddress, $"[UserService - RegisterNewUser] Already exist user with ID: {userInformation.Id}");
                    throw new HttpRequestException($"Already exist user");
                }
            } 
            catch (Exception ex)
            {
                LogTrace(userInformation.UserName, "", ipAddress, $"[UserService - RegisterNewUser] {ex.Message}");
                throw new Exception(ex.Message);
            }

            _stopwatch.Stop();
        }

        public async Task UpdateUser(UserDto userInformation)
        {
            _stopwatch = Stopwatch.StartNew();
            var ipAddress = GetIpAddress();

            try
            {
                var user = _userRepository.GetAll().Where(x => x.Id == userInformation.Id).FirstOrDefault();

                if (user != null)
                {
                    _userRepository.Update(user);

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_appSettings.InternalAPI.BaseAPI);
                        var uri = $"{_appSettings.InternalAPI.BaseAPI}/{_appSettings.InternalAPI.UserAPI.Update}";
                        var content = new StringContent(JsonConvert.SerializeObject(userInformation), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(uri, content);

                        if (response == null)
                        {
                            LogTrace(userInformation.UserName, "", ipAddress, $"[UserService - UpdateUser] Internal Error");
                            throw new Exception("Internal Error");
                        }
                        else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            LogTrace(userInformation.UserName, "", ipAddress, $"[UserService - UpdateUser] Internal Error with status code response: {(int)response.StatusCode}");
                            throw new HttpRequestException($"Internal Error with status code response: {(int)response.StatusCode}");
                        }

                        await _userRepository.SaveChangesAsync();
                    }
                }
                else
                {
                    LogTrace("", "", ipAddress, $"[UserService - UpdateUser] Not found user with ID: {userInformation.Id}");
                    throw new HttpRequestException($"Not found user");
                }
            }
            catch (Exception ex)
            {
                LogTrace(userInformation.UserName, "", ipAddress, $"[UserService - UpdateUser] {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public async Task ChangeUserStatus(Guid userId, bool status)
        {
            _stopwatch = Stopwatch.StartNew();
            var ipAddress = GetIpAddress();

            try
            {
                var user = _userRepository.GetAll().Where(x => x.Id == userId).FirstOrDefault();

                if (user != null)
                {
                    user.IsActive = status;
                    _userRepository.Update(user);

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_appSettings.InternalAPI.BaseAPI);
                        var uri = $"{_appSettings.InternalAPI.BaseAPI}/{_appSettings.InternalAPI.UserAPI.ChangeStatus.Replace("id", userId.ToString())}";
                        var content = new StringContent(status.ToString(), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(uri, content);

                        if (response == null)
                        {
                            LogTrace("", "", ipAddress, $"[UserService - InactiveUser] Internal Error");
                            throw new Exception("Internal Error");
                        }
                        else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            LogTrace("", "", ipAddress, $"[UserService - InactiveUser] Internal Error with status code response: {(int)response.StatusCode}");
                            throw new HttpRequestException($"Internal Error with status code response: {(int)response.StatusCode}");
                        }

                        await _userRepository.SaveChangesAsync();
                    }
                }
                else
                {
                    LogTrace("", "", ipAddress, $"[UserService - InactiveUser] Not found user with ID: {userId}");
                    throw new HttpRequestException($"Not found user");
                }
            }
            catch (Exception ex)
            {
                LogTrace("", "", ipAddress, $"[UserService - InactiveUser] {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        #region Private Methods

        private string GetIpAddress()
        {
            var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

            return remoteIpAddress != null ? remoteIpAddress.ToString() : "";
        }

        private void LogTrace(string? userName, string? clientId, string? ipAddress, string? message)
        {
            _stopwatch.Stop();
            _logger.LogInformation(string.Format(" At {0}. Time spent {1} ", _dateTimeProvider.Now, _stopwatch.Elapsed));
            _logger.LogInformation(string.Format(" UserName: {0} - ClientID: {1} - IpAddress: {2} ", userName, clientId, ipAddress));
            _logger.LogInformation(string.Format(" Message: {0} ", message));
        }

        #endregion
    }
}
