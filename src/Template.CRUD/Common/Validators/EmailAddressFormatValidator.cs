using System.Text.RegularExpressions;

public partial class EmailAddressAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var email = value?.ToString();
        if (email is null)
        {
            return ValidationResult.Success!;
        }
        var validateEmailRegex = MyRegex();

        if (validateEmailRegex.IsMatch(email))
        {
            return ValidationResult.Success!;
        }
        else
        {
            return new ValidationResult("Invalid email address");
        }
    }

    [GeneratedRegex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]
    private static partial Regex MyRegex();
}
