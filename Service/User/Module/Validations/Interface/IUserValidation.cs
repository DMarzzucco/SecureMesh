using User.Module.DTOs;

namespace User.Module.Validations.Interface;

public interface IUserValidation
{
    void ValidationEmail(string email);
    Task ValidateEmailDuplicate(string email);
    Task ValidationUsernameDuplicated(string username);
    void ValidateStructurePassword (string password);
}