namespace PetProject.IdentityServer.Presentation.MiddleWare
{
    public static class MiddleWareExtensions
    {
        public static WebApplication UseCustomMiddleWare(this WebApplication app)
        {
            app.AddGlobalExceptionMiddleWare();

            return app;
        }

        public static WebApplication AddGlobalExceptionMiddleWare(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();

            return app;
        }
    }
}
