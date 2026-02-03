using Application.Dtos.User;
using FluentValidation;
using Presentation.ViewModels.User;

namespace Presentation.Validator
{
  
        public class UpdateRoleViewModelValidator : AbstractValidator<UpdateRoleViewModel>
        {
            public UpdateRoleViewModelValidator()
            {
                RuleFor(x => x.userId).NotEmpty().WithMessage("UserId is required");
                RuleFor(x => x.roleName).NotEmpty().WithMessage("RoleName is required");
            }
        }
    
}
