using System.ComponentModel.DataAnnotations;

namespace RestauranteSync.Application.Common.Behaviors;

public static class ValidationBehavior
{
    public static async Task<T> ValidateAsync<T>(T request, Func<T, Task<T>> next)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "Request cannot be null");

        var validationResults = new List<ValidationResult>();

        var context = new ValidationContext(request);
        
        if (!Validator.TryValidateObject(request, context, validationResults, true))
        {
            var errors = validationResults.Select(v => v.ErrorMessage).ToList();
            throw new ValidationException($"Validation failed: {string.Join(", ", errors)}");
        }

        return await next(request);
    }
}
