using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.PostAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class AddPostCommentCommandHandler :  IRequestHandler<AddPostCommentCommand, OperationResult<PostComment>>
{
    private readonly DataContext _dataContext;

    public AddPostCommentCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<PostComment>> Handle(AddPostCommentCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<PostComment>();

        try
        {
            var post = await _dataContext.Posts.FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);
            
            if (post is null)
            {
                result.AddError(ErrorCode.NotFound, string.Format(PostErrorMessage.PostNotFound, request.PostId));
                return result;
            }
            
            var comment = PostComment.CreatePostComment(request.PostId, request.CommentText, request.UserProfileId);
            
            post.AddPostComment(comment);
            _dataContext.Posts.Update(post);
            await _dataContext.SaveChangesAsync(cancellationToken);

            result.Payload = comment;
        }
        catch (PostCommentNotValidException ex)
        {
            ex.ValidationErrors.ForEach(e => result.AddError(ErrorCode.ValidationError, e));
        }
        catch (Exception ex)
        {
            result.AddError(ErrorCode.UnknownError, ex.Message);
        }

        return result;
    }
}