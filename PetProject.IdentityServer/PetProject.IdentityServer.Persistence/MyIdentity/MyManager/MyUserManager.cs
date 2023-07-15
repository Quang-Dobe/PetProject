using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting;
using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Persistence.MyIdentity.MyManager
{
    public class MyUserManager : UserManager<User>
    {
        private readonly AppSettings _appSettings;

        public MyUserManager(IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccess,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer lookupNormalizer,
            IdentityErrorDescriber identityErrorDescriber,
            IServiceProvider serviceProvider,
            ILogger<MyUserManager> logger,
            AppSettings appSettings) 
            : base(store, optionsAccess, passwordHasher, 
                  userValidators, passwordValidators, lookupNormalizer, 
                  identityErrorDescriber, serviceProvider, logger)
        {
            _appSettings = appSettings;
        }

        public Task SendEmailAccountLockout(string userName, string acoountName)
        {
            var userStore = Store as UserStore;
            if (userStore != null)
            {
                return userStore.SendEmailAccountLockoutAsync(
                    _appSettings.EmailSender.AccountLockoutTitle,
                    _appSettings.EmailSender.FromEmail,
                    _appSettings.EmailSender.FromName,
                    userName,
                    acoountName);
            }
            return Task.CompletedTask;
        }

        public async Task<User?> FindByPhoneNumberAsync(string phoneNumber)
        {
            var userStore = Store as UserStore;
            if (userStore != null)
            {
                return await userStore.FindByPhoneNumberAsync(phoneNumber, CancellationToken);
            }

            return new User();
        }
    }
}
