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
                result.IsError = true;
                result.Errors.Add(new Error
                {
                    Code = ErrorCode.NotFound,
                    Message = $"No Post found for PostId - {request.PostId} with UserProfileId - {request.UserProfileId}"
                });
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
            result.IsError = true;
            ex.ValidationErrors.ForEach(e =>
            {
                result.Errors.Add(new Error
                {
                    Code = ErrorCode.ValidationError,
                    Message = ex.Message
                });
            });
        }
        catch (Exception ex)
        {
            result.IsError = true;
            result.Errors.Add(new Error
            {
                Code = ErrorCode.UnknownError,
                Message = ex.Message
            });
        }

        return result;
    }
}