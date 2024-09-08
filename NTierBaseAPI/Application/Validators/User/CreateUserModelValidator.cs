using Application.Models.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.User
{
    public class CreateUserModelValidator : AbstractValidator<CreateUserModel>
    {
        public CreateUserModelValidator()
        {
            RuleFor(p => p.FirstName)
                .NotEmpty()
                .MaximumLength(UserValidatorConfiguration.MaximumFirstNameLength);

            RuleFor(p => p.LastName)
                .NotEmpty()
                .MaximumLength(UserValidatorConfiguration.MaximumLastNameLength);

            RuleFor(p => p.Password)
                .NotEmpty()
                .Matches(UserValidatorConfiguration.PasswordRegex);
        }
    }
}
