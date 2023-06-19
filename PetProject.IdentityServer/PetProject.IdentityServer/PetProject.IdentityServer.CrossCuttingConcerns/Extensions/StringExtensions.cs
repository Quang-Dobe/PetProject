namespace PetProject.IdentityServer.CrossCuttingConcerns.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsEqualIgnoreCase(this string value, string comparedValue)
        {
            return value.Equals(comparedValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
