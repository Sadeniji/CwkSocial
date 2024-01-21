using CwkSocial.Application.Models;
using MediatR;

namespace CwkSocial.Application.Posts.Commands;

public class DeletePostInteractionCommand : IRequest<OperationResult<bool>>
{
    public Guid PostId { get; set; }
    public Guid UserProfileId { get; set; }
    public Guid InteractionId { get; set; }
}