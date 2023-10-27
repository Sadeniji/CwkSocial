using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.Queries;

public class GetAllUserProfilesQuery : IRequest<IEnumerable<UserProfile>>
{
    
}