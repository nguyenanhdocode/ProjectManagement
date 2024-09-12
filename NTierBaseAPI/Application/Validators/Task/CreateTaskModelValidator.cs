using Application.Models.Task;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Task
{
    public class CreateTaskModelValidator : AbstractValidator<CreateTaskModel>
    {
        public CreateTaskModelValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(TaskValidatorConfiguration.MaximumNameLength);
        }
    }
}
