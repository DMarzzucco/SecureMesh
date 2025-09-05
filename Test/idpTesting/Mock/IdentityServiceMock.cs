using System;
using IdentifyService.JWT.DTOs;
using IdentifyService.Module.DTOs;
using IdentifyService.Server.UMS.DTOs;
using IdentifyService.Server.UMS.Model;

namespace idpTesting.Mock;

public class IdentityServiceMock
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

    public static VerifyTokens TokenVerify => new VerifyTokens
    {
        VerifyEmailOTT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1hcnp6NzdfQGdtYWlsLmNvbSIsInN1YiI6IjIiLCJwdXJwb3NlIjoiZW1haWxfdmVyaWZpY2F0aW9uIiwibmJmIjoxNzUwMzM5NjI4LCJleHAiOjE3NTAzNDAyMjgsImlhdCI6MTc1MDMzOTYyOH0.wvWxzSTn1RDdQ1NtBLbtXjkA0Zj9woJTAWEYRgHttcI",

        VerifySessionOTT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1hcnp6NzdfQGdtYWlsLmNvbSIsInN1YiI6IjIiLCJpcCI6IjguOC44LjguOCIsInVhIjoiRGViYWluIDEyIiwibG9jYXRpb24iOiJWZW5hZG8gVHVlcnRvLCBTYW50YSBGZSwgQXJnZW50aW5hIiwibmJmIjoxNzUwMzM5OTgwLCJleHAiOjE3NTAzNDA1ODAsImlhdCI6MTc1MDMzOTk4MH0.VtfwZPSaOICIxFYjmxG0URHYrZFlZwQDOouBSWJrmFc"
    };

    public static VerifyCodeDTO VerifyCodeDTOMock => new()
    {
        TwoAfCode = "123456",
        Email = "marzz77_@gmail.com"
    };
    public static CreateUserDTO CreateUserDTOMOck => new CreateUserDTO
    {
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "Pr@motheus98",
        Roles = ROLES.ADMIN
    };

    public static SessionModel SessionModelMock => new()
    {
        Id = 2,
        UserId = 4,
        Ip = "8.8.8.8.8",
        UserAgent = "Debain 12",
        Location = "Venado Tuerto, Santa Fe, Argentina",
        IsActive = true
    };

    public static UserModel UserHashPassMock => new()
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

    public static UserModel UserMockEmailTrue => new UserModel
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "AQAAAAIAAYagAAAAEMS4jLBZxqiCLDbX0FXyV3VoeSnq0FBBpYSVdgpFfHw83cBB33cnzomg736FuySfJg==",
        Roles = ROLES.ADMIN
    };
    
    public static NewEmailDTO NewEmailMock => new NewEmailDTO
    {
        Code = "123456",
        Password = "Pr@motheus98",
        NewEmail = "dmarzz_@hotmail.com"
    };

    public static RemoveOwnAccountDTO RemoveOwnAccountDTOMock => new RemoveOwnAccountDTO
    {
        Code = "122343",
        Password = "Pr@motheus98"
    };
    public static UpdatePasswordDTO UpdatePasswordDTOMock => new UpdatePasswordDTO
    {
        Code = "122343",
        OldPassword = "Pr@motheus98",
        NewPassword = "Sr@motheus23"
    };

    public class VerifyTokens
    {
        public required string VerifyEmailOTT { get; set; }

        public required string VerifySessionOTT { get; set; }
    }

    public class SessionsDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Ip { get; set; }
        public required string UserAgent { get; set; }
        public required string Location { get; set; }
    }

}
