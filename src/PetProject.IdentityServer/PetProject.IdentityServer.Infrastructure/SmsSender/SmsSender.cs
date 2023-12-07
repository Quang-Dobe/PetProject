using Azure;
using Azure.Communication.Sms;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Repositories;
using PetProject.IdentityServer.Domain.ThirdPartyServices.SmsSender;
using System.Net.Mail;

namespace PetProject.IdentityServer.Infrastructure.SmsSender
{
    public class SmsSender : ISmsSender
    {
        private readonly string _connectionString;

        private readonly string _fromPhoneNumber;

        private readonly ISmsRepository _smsRepository;

        public SmsSender(SmsSenderConfiguration configuration, ISmsRepository smsRepository)
        {
            _connectionString = configuration.ConnectionString;
            _fromPhoneNumber = configuration.FromPhoneNumber;
            _smsRepository = smsRepository;
        }

        public void SendSms(Sms sms)
        {
            try
            {
                var toPhoneNumbers = new List<string>();
                if (!sms.ToPhoneNumber.IsNullOrEmpty())
                {
                    foreach (var phoneNumber in sms.ToPhoneNumber.Split(';'))
                    {
                        toPhoneNumbers.Add(phoneNumber);
                    }
                }

                var smsClient = new SmsClient(_connectionString);

                Response<IReadOnlyList<SmsSendResult>> sendResult = smsClient.Send(
                    from: _fromPhoneNumber,
                    to: toPhoneNumbers,
                    message: sms.Message
                );
            }
            catch (Exception ex)
            {
                sms.RetryCount += 1;
                _smsRepository.SaveChange();

                throw ex;
            }
        }
    }
}
