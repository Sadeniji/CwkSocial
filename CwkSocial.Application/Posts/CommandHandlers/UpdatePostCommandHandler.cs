using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.PostAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, OperationResult<Post>>
{
    private readonly DataContext _dataContext;

    public UpdatePostCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<Post>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Post>();

        try
        {
            var post = await _dataContext.Posts.FirstOrDefaultAsync(
                p => p.PostId == request.PostId && p.UserProfileId == request.UserProfileId, cancellationToken);
            
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
            
            post.UpdatePostText(request.NewText);
            await _dataContext.SaveChangesAsync(cancellationToken);
            result.Payload = post;
        }
        catch (PostNotValidException ex)
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