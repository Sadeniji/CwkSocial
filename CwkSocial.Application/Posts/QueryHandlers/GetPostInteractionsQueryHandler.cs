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
            var interactions = await _dataContext.Posts
                .Where(p => p.PostId == request.PostId)
                .Include(p => p.Interactions)
                .Include(p => p.UserProfile)
                .SelectMany(p => p.Interactions).ToListAsync(cancellationToken);
            
            var post = await _dataContext.Posts
                .Include(p => p.Interactions)
                .Include(p => p.UserProfile)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

            if (post == null)
            {
                result.AddError(ErrorCode.NotFound, PostErrorMessage.PostNotFound);
                return result;
            }

            //result.Payload = post.Interactions.ToList();
            result.Payload = interactions;
            return result;
        }
        catch (Exception ex)
        {
            result.AddUnKnowError(ex.Message);
            return result;
        }
    }
}