using MagellanTest.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MagellanTest.Models.Validations;

public class ValidateIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
        
        var item = (ItemRecord)((ObjectResult) context.Result).Value;
        
        if (item is null)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Id", $"Cannot find the item. Invalid id provided");
                
            var problemDetails = new ValidationProblemDetails(modelState)
            {
                Status = StatusCodes.Status404NotFound
            };
                
            context.Result = new NotFoundObjectResult(problemDetails);
        }
    }
}