using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PricingService.Controllers;

public sealed class PricingExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ValidationException validation:
                var errors = validation.Errors.GroupBy(error => error.PropertyName)
                    .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(errors));
                break;
            case KeyNotFoundException missing:
                context.Result = new NotFoundObjectResult(new ProblemDetails { Detail = missing.Message, Status = 404 });
                break;
            case ArgumentException invalid:
                context.Result = new BadRequestObjectResult(new ProblemDetails { Detail = invalid.Message, Status = 400 });
                break;
            default:
                return;
        }
        context.ExceptionHandled = true;
    }
}
