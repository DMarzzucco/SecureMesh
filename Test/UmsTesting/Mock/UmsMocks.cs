using System;
using UserManagementService.Server.Users.Model;

namespace UmsTesting.Mock;

public class UmsMocks
{
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

    public static UserDTO UserMockDTO => new()
    {
        Id = 4,
        FullName = "Dario Marzzucco",
        Username = "derkmarzz77",
        Email = "marzz77_@gmail.com"
    };

    public static UpdateOwnRegisterDTO UpdateOwnRegisterDTOMock => new()
    {
        Password = "Pr@motheus98",
        FullName = "Dario Alban Marzzucco",
        Username = "DarMarzRold98"
    };

    public static ForgetPasswordDTO ForgetPasswordDTOMock => new() { Email = "marzz77_@gmail.com" };

    public static PasswordDTO PasswordDTOMock => new() { Password = "Sr@motheus98" };

    public static RolesDTO RolesDTOMock => new() { NewRoles = ROLES.CREATOR};
    public static OttMock TokensOttMock => new()
    {
        RecuperationToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1hcnp6NzdfQGdtYWlsLmNvbSIsInN1YiI6IjIiLCJwdXJwb3NlIjoicGFzc3dvcmRfcmVjdXBlcmF0aW9uIiwibmJmIjoxNzUwMzM5NjI4LCJleHAiOjE3NTAzNDAyMjgsImlhdCI6MTc1MDMzOTYyOH0.79DxWhyYcnwuGAOutuXRxqDlRvr1fpNtPzQAbf0QSq8",

        NewEmailOtt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1hcnp6NzdfQGdtYWlsLmNvbSIsInN1YiI6IjIiLCJzZXNzaW9uSWQiOiIxIiwibmV3X2VtYWlsIjoicmV6QGdtYWlsLmNvbSIsIm5iZiI6MTc1MDMzOTYyOCwiZXhwIjoxNzUwMzQwMjI4LCJpYXQiOjE3NTAzMzk2Mjh9.qVTSRdUBmXroNs8nztPUOJJTKM-HLak5CgrbOfT1NKc"
    };

    public class OttMock
    {
        public required string RecuperationToken { get; set; }
        public required string NewEmailOtt { get; set; }
    }
}
