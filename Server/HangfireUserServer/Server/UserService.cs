using HangfireUserServer.Server.Interfaces;
using UserHangFire.Protos;

namespace HangfireUserServer.Server;

public class UserService(UserHangFireService.UserHangFireServiceClient client) : IUserServices
{
    private readonly UserHangFireService.UserHangFireServiceClient client = client;

    public async Task CountedDeletedAsync(int id)
    {
        var request = new UserRequest { Id = id };
        try
        {
            await this.client.CountedDeletedAsync(request);
        }
        catch (Exception ex) { throw new Exception($"{ex.Message}"); }
    }
}