using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using CwkSocial.Domain.Exceptions;
using CwkSocial.Domain.Validators.PostValidator;

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
    /// <summary>
    /// Create new Post instance
    /// </summary>
    /// <param name="userProfileId">user profile Id</param>
    /// <param name="textContext">Post content</param>
    /// <returns><see cref="Post"/></returns>
    /// <exception cref="PostNotValidException"></exception>
    public static Post CreatePost(Guid userProfileId, string textContext)
    {
        var postValidator = new PostValidator();

        var post = new Post
        {
            UserProfileId = userProfileId,
            TextContent = textContext,
            CreatedDate = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        var postValidationResult = postValidator.Validate(post);

        if (postValidationResult.IsValid)
            return post;

        var postValidationException = new PostNotValidException("Post is invalid");
        postValidationResult.Errors.ForEach(pve => postValidationException.ValidationErrors.Add(pve.ErrorMessage));

        throw postValidationException;
    }

    // public methods
    /// <summary>
    /// Update the Post text
    /// </summary>
    /// <param name="newText">The updated post text</param>
    /// <exception cref="PostNotValidException"></exception>
    public void UpdatePostText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
        {
            var postValidationException = new PostNotValidException("Cannot update post. Post text is not valid.");
            postValidationException.ValidationErrors.Add("The provided text is either null or contains only white space");
            throw postValidationException;
        }
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