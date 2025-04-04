using Security.Cookies.Interfaces;
using Security.JWT.DTOs;

namespace Security.Cookies
{
    public class CookieService : ICookieService
    {
        /// <summary>
        /// Clear Token 
        /// </summary>
        /// <param name="response"></param>
        public void ClearTokenCookies(HttpResponse response)
        {
            SetCookie(response, "Authentication", "", DateTime.UnixEpoch);
            SetCookie(response, "RefreshToken", "", DateTime.UnixEpoch);

        }
        /// <summary>
        /// Set Token Cookies
        /// </summary>
        /// <param name="response"></param>
        /// <param name="tokens"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetTokenCookies(HttpResponse response, TokenPair tokens)
        {
            SetCookie(response, "Authentication", tokens.AccessToken, DateTime.UtcNow.AddHours(2));
            SetCookie(response, "RefreshToken", tokens.RefreshToken, DateTime.UtcNow.AddDays(5));
        }
        /// <summary>
        /// Cookie Template Configuration
        /// </summary>
        private static void SetCookie(
            HttpResponse response,
            string name,
            string value,
            DateTime expiration
            )
        {
            response.Cookies.Append(name, value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expiration,
                SameSite = SameSiteMode.Strict
            });
        }
    }
}
