using Security.JWT.DTOs;

namespace Security.Cookies.Interfaces
{
    public interface ICookieService
    {
        void ClearTokenCookies(HttpResponse response);
        void SetTokenCookies(HttpResponse response, TokenPair tokens);
        void SetCRSFToken(HttpResponse response, string name, string value);
    }
}
