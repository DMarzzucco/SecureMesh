using System.Text.RegularExpressions;
using User.Module.DTOs;
using User.Module.Repository.Interface;
using User.Module.Validations.Interface;
using User.Utils.Exceptions;

namespace User.Module.Validations;

public class UserValidation(IUserRepository repository) : IUserValidation
{
    private readonly IUserRepository _repository = repository;

    public void ValidationCreateUser(CreateUserDTO body)
    {
        var validations = new List<(bool isInvalid, Exception Error)>
        {
        // conflict between repeated values 
        (_repository.ExistisByUsername(body.Username), new ConflictExceptions("This username already exists")),
        (_repository.ExistisByEmail(body.Email), new ConflictExceptions("This email already exists")),
        // Email Validation
        (!Regex.IsMatch(body.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"), new BadRequestExceptions("Invalid email address")),
        //password validations 
        (body.Password.Length < 8, new BadRequestExceptions("Password must be at least 8 characters long.")),
        (!body.Password.Any(char.IsDigit), new BadRequestExceptions("Password must contain at least one digit")),
        (!body.Password.Any(char.IsUpper), new BadRequestExceptions("Password must contain at least one upper case letter")),
        (!body.Password.Any(ch => !char.IsLetterOrDigit(ch)), new BadRequestExceptions("Password must contain at least one special character"))
        };

        var firstError = validations.FirstOrDefault(v => v.isInvalid);
        if (firstError != default)
            throw firstError.Error;
    }
}