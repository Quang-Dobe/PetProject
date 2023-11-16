using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Domain.ThirdPartyServices.SmsSender
{
    public interface ISmsSender
    {
        public void SendSms(Sms sms);
    }
}
