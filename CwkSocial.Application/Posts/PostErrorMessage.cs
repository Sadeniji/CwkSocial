namespace CwkSocial.Application.Posts;

public class PostErrorMessage
{
    public const string PostNotFound = "No post was found for post with postId - {0}";
    public const string UserProfileNotFound = "UserProfile not found for the login user - {0}";
    public const string IdentityUserAlreadyExist = "Provided email address already exists. Cannot register new user";
}