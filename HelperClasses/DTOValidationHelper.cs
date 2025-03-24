using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace EMS.Helpers
{
    public static class DTOValidationHelper
    {
        // Helper method to validate model and return error details
        public static IActionResult ValidateModelState(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                var validationErrors = modelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                return new BadRequestObjectResult(new { message = "Model validation failed.", errors = validationErrors });
            }

            return null; // No validation errors
        }
    }
}
