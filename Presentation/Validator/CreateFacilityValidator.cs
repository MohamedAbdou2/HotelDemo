using FluentValidation;
using Presentation.ViewModels.Facility;

namespace Presentation.Validator;


//public class CreateFacilityValidator : AbstractValidator<CreateFacilityViewModel>
//{
//    public CreateFacilityValidator()
//    {
//        RuleFor(x => x.Name)
//            .NotEmpty().WithMessage("Facility name is required.")
//            .MaximumLength(100).WithMessage("Facility name cannot exceed 100 characters.");

//        RuleFor(x => x.Description)
//            .NotEmpty().WithMessage("Description is required.")
//            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
//    }
//}
