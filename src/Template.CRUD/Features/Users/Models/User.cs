namespace Template.Api.Features.Users.Models;

public class UserModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? Language { get; set; }
    public string? LastName { get; set; }
    public string? PersonalNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PostalCode { get; set; }
    public UserRole Role { get; set; }
    public string? StreetAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum UserRole
{
    administrator,
    support_administrator,
    support,
    user,
    utility
}
