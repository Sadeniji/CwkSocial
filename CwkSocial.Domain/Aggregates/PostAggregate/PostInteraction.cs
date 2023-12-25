using CwkSocial.Domain.Aggregates.UserProfileAggregate;

namespace CwkSocial.Domain.Aggregates.PostAggregate;

public class PostInteraction
{
    private PostInteraction()
    {
        
    }
    public Guid InteractionId { get; private set; }
    public Guid PostId { get; private set; }
    public Guid? UserProfileId { get; private set; }
    public UserProfile UserProfile { get; set; }
    public InteractionType InteractionType { get; private set; }
    
    // Factory Method
    public static PostInteraction CreatePostInteraction(Guid postId, Guid userProfileId, InteractionType interactionType)
    {
        return new PostInteraction
        {
            PostId = postId,
            InteractionType = interactionType,
            UserProfileId = userProfileId
        };
    }
}