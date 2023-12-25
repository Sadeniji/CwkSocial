namespace CwkSocial.Application.Identity;

public abstract class IdentityErrorMessage
{
    public const string NonExistenceIdentityUser = "Unable to find a user with the specified username";
    public const string InCorrectPassword = "The provided password is incorrect";
    public const string IdentityUserAlreadyExist = "Provided email address already exists. Cannot register new user";
}