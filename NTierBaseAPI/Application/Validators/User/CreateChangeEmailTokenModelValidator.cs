using Application.Models.User;
using Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.User
{
    public class CreateChangeEmailTokenModelValidator
        : AbstractValidator<CreateChangeEmailTokenModel>
    {
        private UserManager<AppUser> _userManager;
        public CreateChangeEmailTokenModelValidator(UserManager<AppUser> userManager) 
        {
            _userManager = userManager;

            RuleFor(p => p.NewEmail)
                .Matches(UserValidatorConfiguration.EmailRegex)
                .Must(BeNonExistedEmail)
                .WithMessage("Email is already used");
        }

        private bool BeNonExistedEmail(string email)
        {
            return _userManager.Users.SingleOrDefault(u => u.Email.Equals(email)) == null;
        }
    }
}
