using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.CrossCuttingConcerns.HtmlGenerator;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.Domain.Constants;
using PetProject.IdentityServer.Domain.DTOs;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Repositories;

namespace PetProject.IdentityServer.Persistence.MyIdentity
{
    public class UserStore : IUserStore<User>,
        IUserEmailStore<User>,
        IUserPasswordStore<User>,
        IUserPhoneNumberStore<User>,
        IUserSecurityStampStore<User>,
        IUserLockoutStore<User>,
        IUserTwoFactorStore<User>,
        IUserAuthenticatorKeyStore<User>,
        IUserAuthenticationTokenStore<User>,
        IUserTwoFactorRecoveryCodeStore<User>
    {
        private readonly IUnitOfWork _uow;

        private readonly IdentityDbContext _context;

        private readonly IHtmlGenerator _htmlGenerator;

        private readonly IUserRepository _userRepository;

        private readonly IEmailRepository _emailRepository;

        private readonly IConfiguration _configuration;

        private readonly IDateTimeProvider _dateTimeProvider;

        private const string AuthenticatorStoreLoginProvider = "[AuthenticatorStore]";

        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";

        private const string RecoveryCodeTokenName = "RecoveryCodes";

        public UserStore(IUnitOfWork uow,
            IdentityDbContext context,
            IHtmlGenerator htmlGenerator,
            IUserRepository userRepository,
            IEmailRepository emailRepository,
            IConfiguration configuration,
            IDateTimeProvider dateTimeProvider)
        {
            _uow = uow;
            _context = context;
            _htmlGenerator = htmlGenerator;
            _userRepository = userRepository;
            _emailRepository = emailRepository;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
        }


        public Task<int> CountCodesAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            await _userRepository.AddAsync(user, cancellationToken);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _userRepository.Delete(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));
        }

        public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return _userRepository.GetAll().FirstOrDefaultAsync(x => x.UserName == normalizedEmail);
        }

        public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return _userRepository.GetAll().FirstOrDefaultAsync(x => x.UserName == normalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string?> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string?> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToLower());
        }

        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToLower());
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<string?> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(AuthenticatorKeyTokenName);
        }

        public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnable);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string?> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task<string?> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<string?> GetTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult("");
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(User user, string? email, CancellationToken cancellationToken)
        {
            if (!email.IsNullOrEmpty())
            {
                user.UserName = email;
            }

            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnable = enabled;

            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;

            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(User user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(User user, string? phoneNumber, CancellationToken cancellationToken)
        {
            if (!phoneNumber.IsNullOrEmpty() && phoneNumber.Length == user.PhoneNumber.Length)
            {
                user.PhoneNumber = phoneNumber;
            }

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetTokenAsync(User user, string loginProvider, string name, string? value, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
        {
            if (!userName.IsNullOrEmpty())
            {
                user.UserName = userName;
            }

            return Task.CompletedTask;
        }

        public async Task SendEmailAccountLockoutAsync(string title, string fromEmail, string fromName, string toEmail, string toName)
        {
            var email = new Email
            {
                MailFrom = fromEmail,
                MailTo = toEmail,
                RetryCount = 0,
                MaxRetryCount = 3
            };

            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, $"Templates\\EmailAccountLockout.cshtml");
                var model = new EmailAccountLockoutModel()
                {
                    ToUser = toName,
                    AccountLockoutTimeSpan = Constants.AccountLockoutTimeSpan,
                };

                if (path == null)
                {
                    return;
                }

                email.Body = await _htmlGenerator.GenerateHtmlAsync(path, model);
                email.Subject = title;

                await _emailRepository.AddAsync(email);
                await _uow.SaveChangesAsync();
            } 
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _userRepository.Update(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        { }
    }
}
