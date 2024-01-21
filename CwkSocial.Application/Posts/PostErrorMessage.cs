namespace CwkSocial.Application.Posts;

public class PostErrorMessage
{
    public const string PostNotFound = "No post was found for post with postId - {0}";
    public const string UserProfileNotFound = "No post was found for this user";
    public const string PostDeleteNotPossible = "Only the owner of a post can delete it";
    public const string PostUpdateNotPossible = "Only the owner of a post can update a post";
    public const string PostInteractionNotFound = "Interaction not found - {0}";
    public const string InteractionRemovalNotAuthorized = "Cannot remove interaction as you are not its author - {0}";
    public const string InteractionCreationNotAuthorized = "Cannot create interaction as you are the author of the post - {0}";
    public const string InteractionAlreadyCreatedByUser = "User has created an interaction for this post - {0}";
}