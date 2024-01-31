namespace CwkSocial.Api.Contracts.Identity;

public record IdentityUserProfile(
    string UserName,
    string FirstName,
    string LastName,
    string EmailAddress,
    string Phone,
    DateTime DateOfBirth,
    string CurrentCity,
    string Token);