using System.ComponentModel.DataAnnotations;

namespace Frontend.Services;

public class RegisterModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$", ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DOBMinimumAge(18, ErrorMessage = "You must be at least 18 years old to open an account.")]
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));

    [Required(ErrorMessage = "Street address is required")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Province is required")]
    public string Province { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal code is required")]
    [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Invalid Canadian postal code")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Social Insurance Number is required")]
    [MinLength(9, ErrorMessage = "Social Insurance Number must be 9 characters")]
    public string Sin { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\+\d{10,15}$", ErrorMessage = "Phone number must be in E.164 format (e.g. +14165551234)")]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class DOBMinimumAgeAttribute(int minimumAge) : ValidationAttribute
{
    private readonly int _minimumAge = minimumAge;

    public override bool IsValid(object? value) => value is DateOnly dob && dob <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-_minimumAge));
}