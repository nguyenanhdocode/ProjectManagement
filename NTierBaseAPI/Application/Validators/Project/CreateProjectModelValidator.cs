using Application.Models.Project;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Project
{
    public class CreateProjectModelValidator : AbstractValidator<CreateProjectModel>
    {
        public CreateProjectModelValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(ProjectValidatorConfiguration.MaximumNameLength);
            RuleFor(p => p.BeginDate).NotNull();
            RuleFor(p => p.EndDate).NotNull();
            RuleFor(p => p.EndDate).GreaterThan(p => p.BeginDate);
        }
    }
}
