using System.Text;
using System.Net;
using System.Net.Mail;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.Domain.ThirdPartyServices;
using PetProject.IdentityServer.Domain.ThirdPartyServices.EmailSender;
using PetProject.IdentityServer.Domain.Repositories;

namespace PetProject.IdentityServer.Infrastructure.EmailSenderService
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpHost;

        private readonly int _smtpPort;

        private readonly bool _smtpEnableSsl;

        private readonly string _smtpUserName;

        private readonly string _smtpPassword;

        private readonly IEmailRepository _emailRepository;

        public EmailSender(EmailSenderConfiguration configuration, IEmailRepository emailRepository)
        {
            _smtpHost = configuration.SmtpHost;
            _smtpPort = configuration.SmtpPort;
            _smtpEnableSsl = configuration.SmtpEnableSsl;
            _smtpUserName = configuration.SmtpUserName;
            _smtpPassword = configuration.SmtpPassword;
            _emailRepository = emailRepository;
        }

        public void SendEmail(Domain.Entities.Email email)
        {
            try
            {
                var mailMessage = new MailMessage();

                mailMessage.Subject = email.Subject;
                mailMessage.Body = email.Body;
                mailMessage.From = new MailAddress(email.MailFrom);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = Encoding.GetEncoding("UTF-8");

                if (!email.MailTo.IsNullOrEmpty())
                {
                    foreach (var mailTo in email.MailTo.Split(';'))
                    {
                        mailMessage.To.Add(mailTo);
                    }
                }

                if (!email.MailCc.IsNullOrEmpty())
                {
                    foreach (var mailCC in email.MailCc.Split(';'))
                    {
                        mailMessage.CC.Add(mailCC);
                    }
                }

                var smtpClient = new SmtpClient();
                smtpClient.Host = _smtpHost;
                smtpClient.Port = _smtpPort;
                smtpClient.EnableSsl = _smtpEnableSsl;
                smtpClient.Credentials = new NetworkCredential(_smtpUserName, _smtpPassword);

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                email.RetryCount += 1;
                _emailRepository.SaveChange();

                throw ex;
            }
        }
    }
}
