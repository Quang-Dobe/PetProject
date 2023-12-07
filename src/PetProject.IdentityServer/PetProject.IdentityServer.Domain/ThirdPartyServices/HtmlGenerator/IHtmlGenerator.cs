namespace PetProject.IdentityServer.Domain.ThirdPartyServices
{
    public interface IHtmlGenerator
    {
        Task<string> GenerateHtmlAsync(string path, object model);
    }
}
