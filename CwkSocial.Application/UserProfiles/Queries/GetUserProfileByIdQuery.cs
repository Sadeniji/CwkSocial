using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.Queries;

public class GetUserProfileByIdQuery : IRequest<UserProfile>
{
    public Guid UserProfileId { get; set; }
}