using System.Security.Claims;
using CwkSocial.Application.Identity.Dtos;
using CwkSocial.Application.Models;
using MediatR;

namespace CwkSocial.Application.Identity.Queries;
public record GetCurrentUserQuery(Guid UserProfileId, ClaimsPrincipal ClaimsPrincipal) : IRequest<OperationResult<IdentityUserProfileDto>>;