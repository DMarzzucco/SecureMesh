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

    public static VerifyTokens TokenVerify => new VerifyTokens
    {
        VerifyEmail = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1hcnp6NzdfQGdtYWlsLmNvbSIsInN1YiI6IjIiLCJwdXJwb3NlIjoiZW1haWxfdmVyaWZpY2F0aW9uIiwibmJmIjoxNzUwMzM5NjI4LCJleHAiOjE3NTAzNDAyMjgsImlhdCI6MTc1MDMzOTYyOH0.wvWxzSTn1RDdQ1NtBLbtXjkA0Zj9woJTAWEYRgHttcI",
        
        VerifyNewEmail = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImRtYXJ6el9AaG90bWFpbC5jb20iLCJzdWIiOiIyIiwicHVycG9zZSI6ImVtYWlsX3ZlcmlmaWNhdGlvbiIsIm5iZiI6MTc1MDMzOTgzMCwiZXhwIjoxNzUwMzQwNDMwLCJpYXQiOjE3NTAzMzk4MzB9.C3akMXGBPyT7pDrXYTZpWSGU_qJirlQ_PKnLt-Kl2Fk",

        PasswordRecuperation = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1hcnp6NzdfQGdtYWlsLmNvbSIsInN1YiI6IjIiLCJwdXJwb3NlIjoicGFzc3dvcmRfcmVjdXBlcmF0aW9uIiwibmJmIjoxNzUwMzM5OTgwLCJleHAiOjE3NTAzNDA1ODAsImlhdCI6MTc1MDMzOTk4MH0.2LrmBLPeedYKfKH2oo_v2nKeNtXUmF8sFZe8T8DWKm0"
    };
    public static CreateUserDTO CreateUserDTOMOck => new CreateUserDTO
    {
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "Pr@motheus98",
        Roles = ROLES.ADMIN
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
        EmailVerified = false,
        Roles = ROLES.ADMIN
    };

    public static UserModel UserMockEmailTrue => new UserModel
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com",
        Password = "AQAAAAIAAYagAAAAEMS4jLBZxqiCLDbX0FXyV3VoeSnq0FBBpYSVdgpFfHw83cBB33cnzomg736FuySfJg==",
        EmailVerified = true,
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
    public static NewEmailDTO NewEmailMOck => new NewEmailDTO
    {
        Password = "Pr@motheus98",
        NewEmail = "dmarzz_@hotmail.com"
    };

    public static ForgetPasswordDTO FotgetPasswordMock => new ForgetPasswordDTO { Email = "marzz77_@gmail.com" };

    public static PasswordDTO PasswordDTOMock => new PasswordDTO { Password = "Pr@motheus98" };
    public static PasswordDTO NewPasswordDTOMock => new PasswordDTO { Password = "Sr@motheus23" };

    public class VerifyTokens
    {
        public required string VerifyEmail { get; set; }
        public required string VerifyNewEmail { get; set; }
        public required string PasswordRecuperation { get; set; }
    }
}
