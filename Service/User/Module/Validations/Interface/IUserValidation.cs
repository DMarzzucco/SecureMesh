using User.Module.DTOs;

namespace User.Module.Validations.Interface;

public interface IUserValidation
{
    void ValidationEmail(string email);
    Task ValidateEmailDuplicate(string email);
    Task ValidationDuplicated(CreateUserDTO body);
    void ValidateStructurePassword (string password);
}