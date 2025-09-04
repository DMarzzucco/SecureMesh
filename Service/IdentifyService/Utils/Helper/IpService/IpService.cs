using IdentifyService.Server.UMS.Model;
using IdentifyService.Utils.Helper.IpService.Interfaces;

namespace IdentifyService.Utils.Helper.IpService
{
    public class IpService : IIpService
    {
        public async Task<string> GetCityAsync(string ip)
        {
            using var client = new HttpClient();

            var response = await client.GetFromJsonAsync<IpInfoResponse>($"https://ipinfo.io/{ip}/json");

            return response?.City ?? "Unknowm";
        }
    }
}
