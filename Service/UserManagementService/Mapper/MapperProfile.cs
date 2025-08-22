using System;
using AutoMapper;
using UserManagementService.Server.Users.Model;

namespace UserManagementService.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {

        CreateMap<UserModel, UserDTO>();
    }
}
