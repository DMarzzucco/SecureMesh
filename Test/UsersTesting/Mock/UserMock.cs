using System;
using User;
using User.Module.DTOs;
using User.Module.Model;
namespace UsersTesting.Mock
{
    public static class UsersMock
    {

        public static UserModel UserMock => new UserModel
        {
            Id = 4,
            FullName = "Dario Marzzucco",
            Username = "derkmarzz77",
            Email = "marzz77_@gmail.com",
            Password = "Pr@motheus98",
            Roles = ROLES.Admin
        };

        public static UserModel UserMockBasic => new UserModel
        {
            Id = 5,
            FullName = "Dario Marzzucco",
            Username = "derkmarzz77",
            Email = "marzz77_@gmail.com",
            Password = "Pr@motheus98",
            Roles = ROLES.Basic
        };
        public static UserDTO UserMockDTO => new UserDTO
        {
            Id = 4,
            FullName = "Dario Marzzucco",
            Username = "derkmarzz77",
            Email = "marzz77_@gmail.com",
            Roles = ROLES.Admin
        };
        public static UserModel UserHashPassMock => new UserModel
        {
            Id = 4,
            FullName = "Dario Marzzucco",
            Username = "derkmarzz77",
            Email = "marzz77_@gmail.com",
            Password = "AQAAAAIAAYagAAAAEMS4jLBZxqiCLDbX0FXyV3VoeSnq0FBBpYSVdgpFfHw83cBB33cnzomg736FuySfJg==",
            Roles = ROLES.Admin
        };

        public static CreateUserDTO CreateUserDTOMOck => new CreateUserDTO
        {
            FullName = "Dario Marzzucco",
            Username = "derkmarzz77",
            Email = "marzz77_@gmail.com",
            Password = "Pr@motheus98",
            Roles = ROLES.Admin
        };
        public static UpdateUserDTO UpdateUserDTOMOck => new UpdateUserDTO
        {
            FullName = "Dario Marzzucco",
            Username = "derkmarzz77",
            Email = "marzz77_@gmail.com",
        };
        public static RolesDTO RolesDTOMock => new RolesDTO
        {
            Roles = ROLES.Creator
        };
    }
}
