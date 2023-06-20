namespace PetProject.IdentityServer.CrossCuttingConcerns.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsNullOrEmpty(this Guid? value)
        {
            return value == null || value == Guid.Empty;
        }
    }
}
