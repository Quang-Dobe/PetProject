using Microsoft.AspNetCore.Http;
using System.Text;

namespace PetProject.IdentityServer.Persistence.Extensions
{
    public static class HttpRequestExtension
    {
        public static bool TryGetBasicCredential(this HttpRequest httpRequest, out string clientId, out string clientSecret)
        {
            string authorizations = httpRequest.Headers["Authorization"];

            if (!string.IsNullOrWhiteSpace(authorizations) && authorizations.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    byte[] data = Convert.FromBase64String(authorizations.Substring("Basic ".Length).Trim());
                    string text = Encoding.UTF8.GetString(data);
                    int delimiterIndex = text.IndexOf(':');
                    if (delimiterIndex >= 0)
                    {
                        clientId = text.Substring(0, delimiterIndex);
                        clientSecret = text.Substring(delimiterIndex + 1);

                        return true;
                    }
                }
                catch(FormatException ex)
                {
                    throw new FormatException(ex.Message);
                }
                catch(ArgumentException ex)
                {
                    throw new ArgumentException(ex.Message);
                }
            }

            clientId = null;
            clientSecret = null;

            return false;
        }
    }
}
