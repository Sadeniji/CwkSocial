using CwkSocial.Domain.Exceptions;
using CwkSocial.Domain.Validators.PostValidator;

namespace CwkSocial.Domain.Aggregates.PostAggregate;

public class PostComment
{
    public PostComment()
    {
        
    }
    public Guid CommentId { get; private set; }
    public Guid PostId { get; private set; }
    public string Text { get; private set; }
    public Guid UserProfileId { get; private set; }
    public DateTime DateCreated { get; private set; }
    public DateTime LastModified { get; private set; }
    
    // Factory method
    /// <summary>
    /// Create post comment
    /// </summary>
    /// <param name="postId">Which Id of the post to which the comment belongs</param>
    /// <param name="text">Text content of the comment</param>
    /// <param name="userProfileId">The Id of the user who created the comment</param>
    /// <returns><see cref="PostComment"/></returns>
    /// <exception cref="PostCommentNotValidException">Throw if the data provided for the post comment is not valid</exception>
    public static PostComment CreatePostComment(Guid postId, string text, Guid userProfileId)
    {
        var validator = new PostCommentValidator();
        var postCommentToValidate = new PostComment
        {
            PostId = postId,
            Text = text,
            UserProfileId = userProfileId,
            DateCreated = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        var postCommentValidationResult = validator.Validate(postCommentToValidate);

        if (postCommentValidationResult.IsValid)
            return postCommentToValidate;

        var validationException = new PostCommentNotValidException("Post comment is invalid");

        postCommentValidationResult.Errors.ForEach(pce =>  validationException.ValidationErrors.Add(pce.ErrorMessage));

        throw validationException;
    }
    
    // Public method
    public void UpdateCommentText(string newText)
    {
        Text = newText;
        LastModified = DateTime.UtcNow;
    }
}