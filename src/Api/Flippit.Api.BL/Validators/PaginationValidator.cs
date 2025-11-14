namespace Flippit.Api.BL.Validators;

public static class PaginationValidator
{
    public static void Validate(int page, int pageSize, string? pageParamName = null, string? sizeParamName = null)
    {
        pageParamName ??= nameof(page);
        sizeParamName ??= nameof(pageSize);

        if (page < 1)
            throw new ArgumentException("Page number must be greater than or equal to 1.", pageParamName);

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than or equal to 1.", sizeParamName);
    }
}
