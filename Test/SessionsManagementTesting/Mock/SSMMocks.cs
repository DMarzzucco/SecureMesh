using System;
using SessionsManagement.Module.Model;
using SessionsManagement.Protos;

namespace SessionsManagementTesting.Mock;

public class SSMMocks
{
    public static SessionModel SessionModelMock => new()
    {
        Id = 2,
        UserId = 4,
        Ip = "8.8.8",
        UserAgent = "Debian 12",
        Location = "Venado Tuerto"
    };

    public static SessionsPropsRequest SessionsPropsRequestMock => new()
    {
        UserId = 4,
        Ip = "8.8.8",
        UserAgent = "Debian 12",
        Location = "Venado Tuerto"
    };
}
