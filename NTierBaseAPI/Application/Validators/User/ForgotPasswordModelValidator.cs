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
    public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
    {
        private UserManager<AppUser> _userManager;
        public ForgotPasswordModelValidator(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            RuleFor(p => p.Email).NotEmpty()
                .Must(BeExistedUser)
                .WithMessage("User is not found");
        }

        private bool BeExistedUser(string email)
        {
            return _userManager.Users.SingleOrDefault(p => p.Email.Equals(email)) != null;
        }
    }
}
