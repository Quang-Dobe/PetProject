using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetProject.IdentityServer.Domain.DTOs;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Services;
using PetProject.IdentityServer.Domain.Constants;
using PetProject.IdentityServer.Domain.Repositories;
using PetProject.IdentityServer.Domain.DTOs.User.Request;
using PetProject.IdentityServer.Domain.ThirdPartyServices;
using PetProject.IdentityServer.Persistence.MyIdentity.MyManager;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting;

namespace PetProject.IdentityServer.Application.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IHtmlGenerator _htmlGenerator;

        private readonly IUserRepository _userRepository;

        private readonly IEmailRepository _emailRepository;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly ILogger<UserService> _logger;

        private readonly MyUserManager _userManager;

        private readonly AppSettings _appSettings;

        private Stopwatch _stopwatch;

        public UserService(
            IHtmlGenerator htmlGenerator,
            IUserRepository userRepository,
            IEmailRepository emailRepository,
            IDateTimeProvider dateTimeProvider,
            IHttpContextAccessor httpContextAccessor,
            IPasswordHasher<User> passwordHasher,
            ILogger<UserService> logger,
            MyUserManager userManager,
            AppSettings appSettings)
        {
            _htmlGenerator = htmlGenerator;
            _userRepository = userRepository;
            _emailRepository = emailRepository;
            _dateTimeProvider = dateTimeProvider;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _userManager = userManager;
            _appSettings = appSettings;
        }

        public async Task CreateUser(UserDto userInformation)
        {
            _stopwatch = Stopwatch.StartNew();

            if (userInformation == null
                || userInformation.UserName.IsNullOrEmpty()
                || userInformation.LastName.IsNullOrEmpty()
                || userInformation.FirstName.IsNullOrEmpty()
                || userInformation.PhoneNumber.IsNullOrEmpty())
            {
                throw new BadHttpRequestException("Invalid User Information");
            }

            var ipAddress = GetIpAddress();
            var existedUserWithEmail = await _userManager.FindByEmailAsync(userInformation.UserName);
            var existedUserWithPhoneNumber = await _userManager.FindByPhoneNumberAsync(userInformation.PhoneNumber);

            if (existedUserWithEmail != null)
            {
                LogTrace(userInformation.UserName, "", ipAddress, "[UserService - RegisterNewUser] Existed User");
                throw new BadHttpRequestException("Existed User");
            }
            if (existedUserWithPhoneNumber != null)
            {
                LogTrace(userInformation.UserName, "", ipAddress, "[UserService - RegisterNewUser] Existed PhoneNumber");
                throw new BadHttpRequestException("Existed PhoneNumber");
            }

            var password = GeneratePassword();
            var user = userInformation.ToUser();
            user.Status = true;
            user.LockoutEnable = false;
            user.UserType = Enums.UserType.External;
            user.AccessFailedCount = 0;
            user.IsElectronicSignatureActive = false;
            user.SecurityStamp = GenerateSecurityStamp();
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            await _userRepository.AddAsync(user);

            await GenerateEmailRegisterNewUser(user, password);

            await _emailRepository.SaveChangeAsync();

            _stopwatch.Stop();
        }

        public async Task UpdateUser(UserDto userInformation)
        {
            _stopwatch = Stopwatch.StartNew();

            var ipAddress = GetIpAddress();

            if (userInformation == null)
            {
                LogTrace("", "", ipAddress, "[UserService - UpdateUser] Invalid UserInformation");
                throw new BadHttpRequestException("Invalid UserInformation");
            }
            if (userInformation.Id == null)
            {
                LogTrace(userInformation.UserName, "", ipAddress, "[UserService - UpdateUser] Invalid UserId");
                throw new BadHttpRequestException("Invalid UserId");
            }

            var user = await _userManager.FindByIdAsync(userInformation.Id.ToString());

            if (user == null)
            {
                LogTrace(userInformation.UserName, "", ipAddress, "[UserService - UpdateUser] Invalid UserId");
                throw new BadHttpRequestException("Invalid UserId");
            }

            user.FirstName = userInformation.FirstName ?? user.FirstName;
            user.MiddleName = userInformation.MiddleName ?? user.MiddleName;
            user.LastName = userInformation.LastName ?? user.LastName;
            user.PhoneNumber = userInformation.PhoneNumber ?? user.PhoneNumber;

            if (userInformation.IsChangingPassword)
            {
                if (userInformation.OldPassword != null && userInformation.NewPassword != null)
                {
                    var isValidOldPassword = _passwordHasher.VerifyHashedPassword(user, "", userInformation.OldPassword);

                    if (isValidOldPassword == PasswordVerificationResult.Success)
                    {
                        user.SecurityStamp = GenerateSecurityStamp();
                        user.PasswordHash = _passwordHasher.HashPassword(user, userInformation.NewPassword);
                    }
                    else
                    {
                        LogTrace(user.UserName, "", ipAddress, "[UserService - UpdateUser] Invalid OldPassword");
                        throw new BadHttpRequestException("Invalid OldPassword");
                    }
                }
                else
                {
                    LogTrace(user.UserName, "", ipAddress, "[UserService - UpdateUser] OldPassword or NewPassword is empty");
                    throw new BadHttpRequestException("OldPassword or NewPassword is empty");
                }
            }

            await _userRepository.SaveChangeAsync();

            _stopwatch.Stop();
        }

        public async Task ChangeUserStatus(Guid userId, bool status)
        {
            _stopwatch = Stopwatch.StartNew();

            var ipAddress = GetIpAddress();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                LogTrace("", "", ipAddress, "[UserService - InactiveUser] Invalid UserId");
                throw new BadHttpRequestException("Invalid UserId");
            }

            user.Status = status;

            await _userRepository.SaveChangeAsync();

            _stopwatch.Stop();
        }

        #region Private methods

        private async Task GenerateEmailRegisterNewUser(User user, string password)
        {
            string ipAddress = GetIpAddress();

            var email = new Email
            {
                MailFrom = _appSettings.EmailSender.FromEmail,
                MailTo = user.UserName,
                RetryCount = 0,
                MaxRetryCount = 3
            };

            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, $"Templates\\EmailRegisterNewUser.cshtml");
                var model = new EmailRegisterNewUserModel()
                {
                    ToUser = $"{user.FirstName} {user.LastName}",
                    Password = password,
                };

                if (path == null)
                {
                    return;
                }

                email.Body = await _htmlGenerator.GenerateHtmlAsync(path, model);
                email.Subject = _appSettings.EmailSender.RegisterNewUserTitle;

                await _emailRepository.AddAsync(email);
            }
            catch
            {
                await _userRepository.SaveChangeAsync();

                LogTrace(user.UserName, "", ipAddress, "[UserService - RegisterNewUser] Invalid ClientID / ClientSecret");
                throw new BadHttpRequestException("Invalid ClientID / ClientSecret");
            }
        }

        private string GenerateSecurityStamp()
        {
            byte[] salt = new byte[256 / 8];

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        private string GeneratePassword()
        {
            // Source: https://www.c-sharpcorner.com/article/how-to-generate-a-random-password-in-c-sharp-and-net-core/

            var validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();

            var chars = new char[Constants.GeneratedPasswordLength];
            for (int i = 0; i < Constants.GeneratedPasswordLength; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(chars);
        }

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
