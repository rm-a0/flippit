using FluentValidation;

namespace Flippit.Api.App.Helpers;

public static class ValidationHelper
{
    public static async Task<Dictionary<string, string[]>?> ValidateModelAsync<T>(T model, IValidator<T> validator)
    {
        var validationResult = await validator.ValidateAsync(model);

        if (validationResult.IsValid)
        {
            return null;
        }

        return validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }
}
