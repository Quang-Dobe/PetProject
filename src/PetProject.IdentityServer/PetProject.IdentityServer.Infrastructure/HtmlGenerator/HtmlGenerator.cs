using PetProject.IdentityServer.Domain.ThirdPartyServices;
using RazorLight;

namespace PetProject.IdentityServer.Infrastructure.HtmlGeneratorService
{
    public class HtmlGenerator : IHtmlGenerator
    {
        private readonly IRazorLightEngine _engine;

        public HtmlGenerator(IRazorLightEngine razorLightEngine) 
        {
            _engine = razorLightEngine;
        }

        public async Task<string> GenerateHtmlAsync(string path, object model)
        {
            return await _engine.CompileRenderAsync(path, model);
        }
    }
}
