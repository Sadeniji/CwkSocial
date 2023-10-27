namespace CwkSocial.Api.Contracts.UserProfiles.Responses;

public record BasicInformation(
    string FirstName,
    string LastName,
    string EmailAddress,
    string Phone,
    DateTime DateOfBirth,
    string CurrentCity
);