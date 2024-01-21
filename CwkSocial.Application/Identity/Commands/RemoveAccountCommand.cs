using CwkSocial.Application.Models;
using MediatR;

namespace CwkSocial.Application.Identity.Commands;

public record RemoveAccountCommand(Guid IdentityUserId, Guid RequestorId) : IRequest<OperationResult<bool>>;