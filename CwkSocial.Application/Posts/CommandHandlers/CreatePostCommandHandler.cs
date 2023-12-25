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
                result.AddError(ErrorCode.NotFound, string.Format(PostErrorMessage.UserProfileNotFound, command.UserProfileId));
                return result;
            }
            var post = Post.CreatePost(command.UserProfileId, command.TextContent);
            _dataContext.Posts.Add(post);
            await _dataContext.SaveChangesAsync(cancellationToken);
            result.Payload = post;
        }
        catch (PostNotValidException ex)
        {
            ex.ValidationErrors.ForEach(e =>
            {
                result.AddError(ErrorCode.ValidationError, e);
            });
        }
        catch (Exception ex)
        {
            result.AddError(ErrorCode.UnknownError, ex.Message);
        }
        return result;
    }
}