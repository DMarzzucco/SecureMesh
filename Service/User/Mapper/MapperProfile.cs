using AutoMapper;
using User.Module.DTOs;
using User.Module.Model;

namespace User.Mapper;

public class MapperProfile:Profile
{
    public MapperProfile()
    {
        CreateMap<CreateUserDTO, UserModel>();
        CreateMap<UpdateUserDTO, UserModel>();
        
        CreateMap<UpdateOwnUserDTO, UserModel>()
            .ForMember(d=> d.Password, opt=> opt.Ignore());

        CreateMap<UserModel, UserDTO>();
    }
}