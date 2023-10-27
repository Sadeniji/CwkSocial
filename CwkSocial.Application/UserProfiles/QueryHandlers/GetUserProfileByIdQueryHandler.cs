using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.QueryHandlers;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfile>
{
    private readonly DataContext _context;

    public GetUserProfileByIdQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<UserProfile> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserProfileId == request.UserProfileId,
            cancellationToken);
    }
}