using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.QueryHandlers;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, OperationResult<UserProfile>>
{
    private readonly DataContext _context;

    public GetUserProfileByIdQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<UserProfile>> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<UserProfile>();
        var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserProfileId == request.UserProfileId,
            cancellationToken);
        
        if (userProfile is null)
        {
            result.IsError = true;
            result.Errors.Add(new Error
            {
                Code = ErrorCode.NotFound,
                Message = $"No UserProfile found for userProfileId - {request.UserProfileId}"
            });
            return result;
        }

        result.Payload = userProfile;
        return result;
    }
}