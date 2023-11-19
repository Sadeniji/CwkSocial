using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.QueryHandlers;

internal class GetAllUserProfilesQueryHandler : IRequestHandler<GetAllUserProfilesQuery, OperationResult<IEnumerable<UserProfile>>>
{
    private readonly DataContext _dataContext;

    public GetAllUserProfilesQueryHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<IEnumerable<UserProfile>>> Handle(GetAllUserProfilesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<IEnumerable<UserProfile>>
        {
            Payload = await _dataContext.UserProfiles.ToListAsync(cancellationToken)
        };
        return result;
    }
}