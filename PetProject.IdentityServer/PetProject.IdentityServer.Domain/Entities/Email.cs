using PetProject.IdentityServer.Domain.Entities.BaseEntity;
using PetProject.IdentityServer.Enums;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class Email : AggregateEntity<Guid>
    {
        public string? Subject { get; set; }

        public EmailType EmailType { get; set; }

        public string? MailFrom { get; set; }

        public string? MailTo { get; set; }

        public string? MailCc { get; set; }

        public string? MailBcc { get; set; }

        public string? Body { get; set; }

        public bool Status { get; set; }

        public string? Note { get; set; }

        public int RetryCount { get; set; }

        public int MaxRetryCount { get; set; }
    }
}
