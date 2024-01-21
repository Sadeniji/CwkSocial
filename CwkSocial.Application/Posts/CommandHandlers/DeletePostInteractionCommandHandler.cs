using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class DeletePostInteractionCommandHandler : IRequestHandler<DeletePostInteractionCommand, OperationResult<bool>>
{
    private readonly DataContext _dataContext;

    public DeletePostInteractionCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<bool>> Handle(DeletePostInteractionCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        try
        {
            var post = await _dataContext.Posts
                .Include(i => i.Interactions)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

            if (post is null)
            {
                result.AddError(ErrorCode.NotFound, string.Format(PostErrorMessage.PostNotFound, request.PostId));
                return result;
            }

            var interaction = post.Interactions.FirstOrDefault(i => i.InteractionId == request.InteractionId);

            if (interaction is null)
            {
                result.AddError(ErrorCode.NotFound, string.Format(PostErrorMessage.PostInteractionNotFound, request.InteractionId));
                return result;
            }

            if (interaction.UserProfileId != request.UserProfileId)
            {
                result.AddError(ErrorCode.InteractionRemovalNotAuthorized, 
                    string.Format(PostErrorMessage.InteractionRemovalNotAuthorized, request.InteractionId));
                return result;
            }
            post.RemoveInteraction(interaction);
            _dataContext.Posts.Update(post);
            await _dataContext.SaveChangesAsync(cancellationToken);
            result.Payload = true;
            return result;
        }
        catch (Exception ex)
        {
            result.AddUnKnowError(ex.Message);
            return result;
        }
    }
}