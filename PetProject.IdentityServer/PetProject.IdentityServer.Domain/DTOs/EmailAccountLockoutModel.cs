namespace PetProject.IdentityServer.Domain.DTOs
{
    public class EmailAccountLockoutModel
    {
        public string ToUser { get; set; }

        public int AccountLockoutTimeSpan { get; set; }
    }
}
