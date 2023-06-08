namespace PetProject.IdentityServer.Domain.DTOs
{
    public class EmailLockoutResultDto
    {
        public string? AccountName { get; set; }

        public int AccountLockoutTimeSpan { get; set; }
    }
}