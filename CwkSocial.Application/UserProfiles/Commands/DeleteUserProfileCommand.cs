using MediatR;

namespace CwkSocial.Application.UserProfiles.Commands;

public class DeleteUserProfileCommand : IRequest
{
    public Guid UserProfileId { get; set; } 
}