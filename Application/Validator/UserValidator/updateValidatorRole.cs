using Application.Dtos.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validator.UserValidator
{
    public class updateValidatorRole : AbstractValidator<UpdateRoleDto>
    {
        public updateValidatorRole() 
        { 
         RuleFor(x => x.userId).NotEmpty().WithMessage("UserId is required");
         RuleFor(x => x.roleName).NotEmpty().WithMessage("RoleName is required");
        }
    }
}
