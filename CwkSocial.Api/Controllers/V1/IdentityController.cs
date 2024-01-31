
using AutoMapper;
using CwkSocial.Api.Contracts.Identity;
using CwkSocial.Api.Extensions;
using CwkSocial.Api.Filters;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Identity.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
public class IdentityController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public IdentityController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [Route(ApiRoutes.Identity.Registration)]
    [ValidateModel]
    public async Task<IActionResult> Register(UserRegistration registration, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<RegisterIdentityCommand>(registration);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsError ? 
            HandleErrorResponse(result.Errors) : 
            Ok(_mapper.Map<AuthenticationResult>(result.Payload));

        // var authenticationResult = new AuthenticationResult
        // {
        //     Token = result.Payload
        // };
        // return Ok(authenticationResult); 
    }

    [HttpPost]
    [Route(ApiRoutes.Identity.Login)]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] Login login, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<LoginCommand>(login);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsError ? 
            HandleErrorResponse(result.Errors) : 
            Ok(_mapper.Map<AuthenticationResult>(result.Payload));
        // if (result.IsError)
        // {
        //     return HandleErrorResponse(result.Errors);
        // }
        //
        // var authenticationResult = new AuthenticationResult
        // {
        //     Token = result.Payload
        // };
        // return Ok(authenticationResult);
    }

    [HttpDelete]
    [Route(ApiRoutes.Identity.IdentityById)]
    [ValidateGuid("identityUserId")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteAccount(string identityUserId, CancellationToken cancellationToken)
    {
        var identityUserIdInGuid = Guid.Parse(identityUserId);
        var requestorId = HttpContext.GetIdentityIdClaimValue();

        var deleteCommand = new RemoveAccountCommand(identityUserIdInGuid, requestorId);

        var response = await _mediator.Send(deleteCommand, cancellationToken);
        
        return response.IsError
            ? HandleErrorResponse(response.Errors)
            : Ok(response.Payload); 
    }   

    [HttpGet]
    [Route(ApiRoutes.Identity.CurrentUser)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CurrentUser(CancellationToken cancellationToken)
    {
        var userProfileId = HttpContext.GetUserProfileIdClaimValue();
        var claimsPrincipal = HttpContext.User;

        var query = new GetCurrentUserQuery(userProfileId, claimsPrincipal);
        var result = await _mediator.Send(query, cancellationToken);
        
        return result.IsError ? 
            HandleErrorResponse(result.Errors) : 
            Ok(_mapper.Map<IdentityUserProfile>(result.Payload));
        return Ok();
    }
}