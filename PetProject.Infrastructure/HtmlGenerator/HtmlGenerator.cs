using PetProject.IdentityServer.CrossCuttingConcerns.HtmlGenerator;
using RazorLight;

namespace PetProject.IdentityServer.Infrastructure.HtmlGeneratorServices
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
