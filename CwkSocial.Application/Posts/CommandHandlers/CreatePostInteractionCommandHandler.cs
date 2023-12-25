using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class CreatePostInteractionCommandHandler : IRequestHandler<CreatePostInteractionCommand, OperationResult<PostInteraction>>
{
    private readonly DataContext _dataContext;

    public CreatePostInteractionCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<PostInteraction>> Handle(CreatePostInteractionCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<PostInteraction>();

        try
        {
            var post = await _dataContext.Posts.FirstOrDefaultAsync(
                p => p.PostId == request.PostId && p.UserProfileId == request.UserProfileId, cancellationToken);

            if (post is null)
            {
                result.AddError(ErrorCode.NotFound, string.Format(PostErrorMessage.PostNotFound, request.PostId));
                return result;
            }

            var newInteraction = PostInteraction.CreatePostInteraction(request.PostId, request.UserProfileId, request.InteractionType);
            
            post.AddInteraction(newInteraction);

            _dataContext.Posts.Update(post);
            await _dataContext.SaveChangesAsync(cancellationToken);

            result.Payload = newInteraction;
            return result;
        }
        catch (Exception ex)
        {
            result.AddUnKnowError(ex.Message);
            return result;
        }
    }
}