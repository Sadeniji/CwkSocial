using AutoMapper;
using CwkSocial.Api.Contracts.UserProfiles.Requests;
using CwkSocial.Api.Contracts.UserProfiles.Responses;
using CwkSocial.Api.Filters;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Application.UserProfiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserProfilesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    
    public UserProfilesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllProfiles(CancellationToken cancellationToken)
    {
        var query = new GetAllUserProfilesQuery();
        var response = await _mediator.Send(query, cancellationToken);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(_mapper.Map<List<UserProfileResponse>>(response.Payload));
    }
    
    // This is now included in Registering user identity
    // [HttpPost]
    // [ValidateModel]
    // public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateUpdate profile, CancellationToken cancellationToken)
    // {
    //     var command = _mapper.Map<CreateUserCommand>(profile);
    //
    //     var response = await _mediator.Send(command, cancellationToken);
    //
    //     if (response.IsError)
    //     {
    //         return HandleErrorResponse(response.Errors);
    //     }
    //     var userProfile = _mapper.Map<UserProfileResponse>(response.Payload);
    //
    //     return CreatedAtAction(nameof(GetUserProfileById), new { id = userProfile.UserProfileId }, userProfile);
    // }

    [HttpGet]
    [ValidateGuid("id")]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    public async Task<IActionResult> GetUserProfileById(string id, CancellationToken cancellationToken)
    {
        var query = new GetUserProfileByIdQuery{ UserProfileId = Guid.Parse(id)};

        var response = await _mediator.Send(query, cancellationToken);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(_mapper.Map<UserProfileResponse>(response.Payload));
    }

    [HttpPatch]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    [ValidateModel]
    [ValidateGuid("id")]
    public async Task<IActionResult> UpdateUserProfile(string id, [FromBody] UserProfileCreateUpdate updatedProfile, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateUserProfileCommand>(updatedProfile);
        command.UserProfileId = Guid.Parse(id);
        var response = await _mediator.Send(command, cancellationToken);

        return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
    }

    [HttpDelete]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteUserProfile(string id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserProfileCommand { UserProfileId = Guid.Parse(id) };
        var response = await _mediator.Send(command, cancellationToken);
        
        return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
    }
}