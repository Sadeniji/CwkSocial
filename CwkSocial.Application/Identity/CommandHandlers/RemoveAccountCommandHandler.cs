using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles;
using CwkSocial.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class RemoveAccountCommandHandler : IRequestHandler<RemoveAccountCommand, OperationResult<bool>>
{
    private readonly DataContext _dataContext;

    public RemoveAccountCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<bool>> Handle(RemoveAccountCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        try
        {
            var identityUser = await _dataContext.Users
                .FirstOrDefaultAsync(iu => iu.Id == request.IdentityUserId.ToString(), cancellationToken);

            if (identityUser == null)
            {
                result.AddError(ErrorCode.IdentityUserDoesNotExist, IdentityErrorMessage.NonExistenceIdentityUser);
                return result;
            }

            var userProfile = await _dataContext.UserProfiles
                .FirstOrDefaultAsync(up => up.IdentityId == request.IdentityUserId.ToString(), cancellationToken);

            if (userProfile is null)
            {
                result.AddError(ErrorCode.NotFound, UserProfileErrorMessage.NoUserProfileFound);
                return result;
            }

            if (identityUser.Id != request.RequestorId.ToString())
            {
                result.AddError(ErrorCode.UnAuthorizedAccountRemoval, IdentityErrorMessage.UnAuthorizedAccountRemoval);
                return result;
            }

            var userPosts = _dataContext.Posts
                .Include(p => p.Interactions.Where(l => l.UserProfileId == userProfile.UserProfileId));
            
            foreach (var userPost in userPosts)
            {
                var interactions = userPost.Interactions.Where(p => p.UserProfileId == userProfile.UserProfileId).ToList();
                foreach (var interaction in interactions)
                {
                    userPost.RemoveInteraction(interaction);
                }
            }
            _dataContext.Posts.UpdateRange(userPosts);
            _dataContext.UserProfiles.Remove(userProfile);
            _dataContext.Users.Remove(identityUser);
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