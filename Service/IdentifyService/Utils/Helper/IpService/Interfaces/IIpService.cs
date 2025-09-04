namespace IdentifyService.Utils.Helper.IpService.Interfaces
{
    public interface IIpService
    {
        Task<string> GetCityAsync(string ip);
    }
}
