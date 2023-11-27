using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.PostAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, OperationResult<Post>>
{
    private readonly DataContext _dataContext;

    public CreatePostCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<Post>> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Post>();
        try
        {
            var userProfileExist =
                await _dataContext.UserProfiles.AnyAsync(us => us.UserProfileId == command.UserProfileId,
                    cancellationToken);
            
            if (!userProfileExist)
            {
                result.IsError = true;
                result.Errors.Add(new Error
                {
                    Code = ErrorCode.NotFound,
                    Message = $"No UserProfile found for userProfileId - {command.UserProfileId}"
                });
                return result;
            }
            var post = Post.CreatePost(command.UserProfileId, command.TextContent);
            _dataContext.Posts.Add(post);
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