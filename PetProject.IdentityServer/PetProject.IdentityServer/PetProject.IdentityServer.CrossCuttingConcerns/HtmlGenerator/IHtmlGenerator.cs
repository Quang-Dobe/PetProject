namespace PetProject.IdentityServer.CrossCuttingConcerns.HtmlGenerator
{
    public interface IHtmlGenerator
    {
        Task<string> GenerateHtmlAsync(string path, object model);
    }
}
