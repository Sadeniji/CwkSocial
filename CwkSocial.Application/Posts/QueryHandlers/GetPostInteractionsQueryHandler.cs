using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.QueryHandlers;

public class GetPostInteractionsQueryHandler : IRequestHandler<GetPostInteractionsQuery, OperationResult<List<PostInteraction>>>
{
    private readonly DataContext _dataContext;

    public GetPostInteractionsQueryHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<List<PostInteraction>>> Handle(GetPostInteractionsQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<List<PostInteraction>>();

        try
        {
            // var interactions = await _dataContext.Posts
            //     .Where(p => p.PostId == request.PostId)
            //     .Include(p => p.Interactions)
            //     .Include(p => p.UserProfile)
            //     .SelectMany(p => p.Interactions).ToListAsync(cancellationToken);
            
            var post = await _dataContext.Posts
                .Include(p => p.Interactions)
                .Include(p => p.UserProfile)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

            if (post == null)
            {
                result.AddError(ErrorCode.NotFound, PostErrorMessage.PostNotFound);
                return result;
            }

            var userIds = post.Interactions.Where(x => x.UserProfile == null && x.UserProfileId != null).
                Select(p => p.UserProfileId).ToList();

            if (userIds.Any())
            {
                var userProfiles = await _dataContext.UserProfiles
                    .Where(u => userIds.Contains(u.UserProfileId))
                    .ToListAsync(cancellationToken);

                foreach (var interaction in post.Interactions)
                {
                    interaction.UserProfile = interaction.UserProfile == null
                        ? userProfiles.FirstOrDefault(up => up.UserProfileId == interaction.UserProfileId)
                        : interaction.UserProfile;
                }
            }
            
            //result.Payload = post.Interactions.ToList();
            result.Payload = post.Interactions.ToList();
            return result;
        }
        catch (Exception ex)
        {
            result.AddUnKnowError(ex.Message);
            return result;
        }
    }
}