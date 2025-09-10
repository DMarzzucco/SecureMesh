using System;
using User;
using User.Module.DTOs;
using User.Module.Model;

namespace UserDomainTest.Mock;

public class UserDomainMocks
{
    public static UserModel UserHashPassMock => new()
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "AQAAAAIAAYagAAAAEMS4jLBZxqiCLDbX0FXyV3VoeSnq0FBBpYSVdgpFfHw83cBB33cnzomg736FuySfJg==",
        Roles = ROLES.Admin
    };
    public static UserModel UserMock => new()
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "Pr@motheus98",
        Roles = ROLES.Admin
    };
    public static AuthUserResponse AuthUserResponseMock => new()
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "Pr@motheus98",
        Roles = ROLES.Admin
    };
    public static UserModel UserNotAdminMock => new()
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "Pr@motheus98",
        Roles = ROLES.Creator
    };

    public static CreateUserDTO CreateUserDTOMOck => new()
    {
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "Pr@motheus98",
        Roles = ROLES.Admin
    };

    public static UpdateOwnUserDTO UpdateOwnUserDTOMock => new()
    {
        Password = "Pr@motheus98",
        FullName = "Dario Alban Marzzucco",
        Username = "DarMarzRold98"
    };
}
