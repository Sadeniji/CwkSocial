
using AutoMapper;
using CwkSocial.Api.Contracts.Identity;
using CwkSocial.Api.Filters;
using CwkSocial.Application.Identity.Commands;
using MediatR;
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

        if (result.IsError)
        {
            return HandleErrorResponse(result.Errors);
        }

        var authenticationResult = new AuthenticationResult
        {
            Token = result.Payload
        };
        return Ok(authenticationResult); 
    }

    [HttpPost]
    [Route(ApiRoutes.Identity.Login)]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] Login login, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<LoginCommand>(login);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            return HandleErrorResponse(result.Errors);
        }

        var authenticationResult = new AuthenticationResult
        {
            Token = result.Payload
        };
        return Ok(authenticationResult);
    }
}