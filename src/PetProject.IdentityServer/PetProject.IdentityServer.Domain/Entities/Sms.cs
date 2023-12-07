using PetProject.IdentityServer.Domain.Entities.BaseEntity;

namespace PetProject.IdentityServer.Domain.Entities
{
    public class Sms : AggregateEntity<Guid>
    {
        public string ToPhoneNumber { get; set; }

        public string Message { get; set; }

        public bool Status { get; set; }

        public string? Note { get; set; }

        public int RetryCount { get; set; }

        public int MaxRetryCount { get; set; }
    }
}
