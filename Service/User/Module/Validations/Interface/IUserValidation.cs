using User.Module.DTOs;

namespace User.Module.Validations.Interface;

public interface IUserValidation
{
    void ValidationCreateUser(CreateUserDTO body);
}