using System;
using Security.JWT.DTOs;
using Security.Module.DTOs;
using Security.Server.DTOs;
using Security.Server.Model;

namespace SecurityTesting.Mock;

public class SecurityMock
{
    public static LoginDTO LoginDTOMock => new LoginDTO
    {
        Username = "derkmarzz77",
        Password = "Pr@motheus98"
    };
    public static TokenPair TokenMock => new TokenPair
    {
        AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwicm9sIjoiMSIsIm5iZiI6MTczNTM5MDkzMiwiZXhwIjoxNzM1NTYzNzMyLCJpYXQiOjE3MzUzOTA5MzJ9.fxCAmD20OHRbD28D5PhuVkLkidcySTblRdT0geFQfO4",
        RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwicm9sIjoiMSIsIm5iZiI6MTczNTM5MDkzMiwiZXhwIjoxNzM1ODIyOTMyLCJpYXQiOjE3MzUzOTA5MzJ9.7WoceqK9cqsQvs6KEAymuY8nyU4ElAV_bUBFU8WEacs",
        RefreshHasherToken = "$2a$11$4oaZ9eM55kz2WkDnazw7s.Uh66Pu/raUH0tue3qqRPd1V6NEJcf/."
    };
    public static UserModel UserHashPassMock => new UserModel
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "AQAAAAIAAYagAAAAEMS4jLBZxqiCLDbX0FXyV3VoeSnq0FBBpYSVdgpFfHw83cBB33cnzomg736FuySfJg==",
        Roles = ROLES.ADMIN
    };
    public static UserModel UserMock => new UserModel
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "Pr@motheus98",
        Roles = ROLES.ADMIN
    };
    public static UserDTO UserMockDTO => new UserDTO
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Roles = ROLES.ADMIN
    };

}
