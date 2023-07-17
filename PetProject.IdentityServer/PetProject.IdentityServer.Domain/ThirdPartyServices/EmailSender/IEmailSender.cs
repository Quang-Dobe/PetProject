using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Domain.ThirdPartyServices
{
    public interface IEmailSender
    {
        void SendEmail(Email email);
    }
}
