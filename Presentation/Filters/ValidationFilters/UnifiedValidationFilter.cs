using Examination_System.Models.Enums;
using Examination_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelDemo.ValidationFilters
{
    public class UnifiedValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            ValidateGuid(context);
            if (context.Result != null) return; 

            ValidateModelState(context);
        }

        private void ValidateModelState(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

                var response = ResponseViewModel<object>.Failure(
                    ErrorCode.ValidationError,
                    string.Join("\n", errors)
                );

                context.Result = new BadRequestObjectResult(response);
            }
        }

        private void ValidateGuid(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is Guid guidValue)
                {
                    if (guidValue == Guid.Empty)
                    {
                        var response = ResponseViewModel<object>.Failure(
                            ErrorCode.BadRequest,
                            $"{argument.Key} cannot be empty."
                        );
                        context.Result = new BadRequestObjectResult(response);
                        return;
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {      
        }
    }
}
