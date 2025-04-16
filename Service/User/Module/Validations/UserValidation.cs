using System.Text.RegularExpressions;
using System.Threading.Tasks;
using User.Module.DTOs;
using User.Module.Repository.Interface;
using User.Module.Validations.Interface;
using User.Utils.Exceptions;

namespace User.Module.Validations;

public class UserValidation(IUserRepository repository) : IUserValidation
{
    private readonly IUserRepository _repository = repository;

    /// <summary>
    /// Validation Email 
    /// </summary>
    /// <param name="email"></param>
    public void ValidationEmail(string email)
    {
        var disposableDomains = new[] { "gmail.com", "hotmail.com", "outlook.com", "icloud.com", "yahoo.com" };
        var emailDomains = email.Split('@')[1];

        var validations = new List<(bool isInvalid, Exception Error)>
        {
            (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"), new BadRequestExceptions("Invalid email address")),
            (email.Length > 320, new BadRequestExceptions ("Email is too long")),
            (!disposableDomains.Contains(emailDomains), new BadRequestExceptions ("Disposable email domains are not allowed"))
        };

        var firstError = validations.FirstOrDefault(v => v.isInvalid);
        if (firstError != default)
            throw firstError.Error;
    }

    /// <summary>
    /// Validation Duplicated
    /// </summary>
    /// <param name="body"></param>
    public async Task ValidationDuplicated(CreateUserDTO body)
    {
        var normalizedUsername = body.Username.Trim().ToLowerInvariant();
        var normalizedEmail = body.Email.Trim().ToLowerInvariant();

        var validation = new List<(bool isInvalid, Exception Error)>
        {
           (await _repository.ExistisByUsername(normalizedUsername), new ConflictExceptions("This username already exists")),
           (await _repository.ExistisByEmail(normalizedEmail), new ConflictExceptions("This email already exists")),
        };
        var firstError = validation.FirstOrDefault(v => v.isInvalid);
        if (firstError != default)
            throw firstError.Error;
    }

    /// <summary>
    /// Validate Strcture Password
    /// </summary>
    /// <param name="password"></param>
    public void ValidateStructurePassword(string password)
    {
        var validation = new List<(bool isInvalid, Exception Error)>
        {
            (password.Length < 8, new BadRequestExceptions("Password must be at least 8 characters long.")),
            (!password.Any(char.IsDigit), new BadRequestExceptions("Password must contain at least one digit")),
            (!password.Any(char.IsUpper), new BadRequestExceptions("Password must contain at least one upper case letter")),
            (!password.Any(ch => !char.IsLetterOrDigit(ch)), new BadRequestExceptions("Password must contain at least one special character"))
        };
        var firstError = validation.FirstOrDefault(v => v.isInvalid);
        if (firstError != default)
            throw firstError.Error;
    }
}