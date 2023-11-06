﻿using AutoMapper;
using CwkSocial.Api.Contracts.Common;
using CwkSocial.Api.Contracts.UserProfiles.Requests;
using CwkSocial.Api.Contracts.UserProfiles.Responses;
using CwkSocial.Application.Enums;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
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
    public async Task<IActionResult> GetAllProfiles()
    {
        var query = new GetAllUserProfilesQuery();
        var response = await _mediator.Send(query);
        var profiles = _mapper.Map<List<UserProfileResponse>>(response);
        return Ok(profiles);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateUpdate profile)
    {
        var command = _mapper.Map<CreateUserCommand>(profile);

        var response = await _mediator.Send(command);

        var userProfile = _mapper.Map<UserProfileResponse>(response);
        return CreatedAtAction(nameof(GetUserProfileById), new { id = response.UserProfileId }, userProfile);
    }

    [HttpGet()]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    public async Task<IActionResult> GetUserProfileById(string id)
    {
        var query = new GetUserProfileByIdQuery{ UserProfileId = Guid.Parse(id)};

        var response = await _mediator.Send(query);

        if (response is null)
        {
            return NotFound($"No user with profile Id {id} found.");
        }
        var userProfile = _mapper.Map<UserProfileResponse>(response);
        return Ok(userProfile);
    }

    [HttpPatch]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    public async Task<IActionResult> UpdateUserProfile(string id, [FromBody] UserProfileCreateUpdate updatedProfile)
    {
        var command = _mapper.Map<UpdateUserProfileCommand>(updatedProfile);
        command.UserProfileId = Guid.Parse(id);
        var response = await _mediator.Send(command);

        return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
    }

    [HttpDelete]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    public async Task<IActionResult> DeleteUserProfile(string id)
    {
        var command = new DeleteUserProfileCommand { UserProfileId = Guid.Parse(id) };
        await _mediator.Send(command);
        return NoContent();
    }
}