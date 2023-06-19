using CwkSocial.Domain.Aggregates.UserProfileAggregate;

namespace CwkSocial.Domain.Aggregates.PostAggregate;

public class Post
{
    private readonly List<PostComment> _comments = new List<PostComment>();
    private readonly List<PostInteraction> _interactions = new List<PostInteraction>();
    private Post()
    {
    }
    public Guid PostId { get; private set; }
    public Guid UserProfileId { get; private set; }
    public string TextContent { get; private set; } // Later support media document
    public DateTime CreatedDate { get; private set; }
    public DateTime LastModified { get; private set; }
    public IEnumerable<PostComment> Comments => _comments;
    public IEnumerable<PostInteraction> Interactions => _interactions;
    public UserProfile UserProfile { get; private set; }

    // Factory method
    public static Post CreatePost(Guid userProfileId, string textContext)
    {
        return new Post
        {
            UserProfileId = userProfileId,
            TextContent = textContext,
            CreatedDate = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
    }

    // public methods
    public void UpdatePostText(string newText)
    {
        TextContent = newText;
        LastModified = DateTime.UtcNow;
    }

    public void AddPostComment(PostComment newComment)
    {
        _comments.Add(newComment);
    }

    public void RemoveComment(PostComment commentToRemove)
    {
        _comments.Remove(commentToRemove);
    }
    
    public void AddInteraction(PostInteraction newInteraction)
    {
        _interactions.Add(newInteraction);
    }
    
    public void RemoveInteraction(PostInteraction interactionToRemove)
    {
        _interactions.Remove(interactionToRemove);
    }
}