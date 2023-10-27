namespace CwkSocial.Api.Contracts.UserProfiles.Requests;

public record UserProfileCreateUpdate(string FirstName,
    string LastName,
    string EmailAddress,
    string Phone,
    DateTime DateOfBirth,
    string CurrentCity);