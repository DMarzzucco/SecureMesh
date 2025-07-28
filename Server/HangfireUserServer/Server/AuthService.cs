using HangfireUserServer.Server.Interfaces;
using AuthHangFire.Proto;

namespace HangfireUserServer.Server;

public class AuthService(AuthHangFireService.AuthHangFireServiceClient client) : IAuthServices
{
    private readonly AuthHangFireService.AuthHangFireServiceClient client = client;

    public async Task CountedDeletedAsync(int id)
    {
        var request = new AuthRequest { Id = id };
        try
        {
            await this.client.CountedDeletedAsync(request);
        }
        catch (Exception ex) { throw new Exception($"{ex.Message}"); }
    }
}