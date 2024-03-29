﻿using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.Queries;

public class GetUserProfileByIdQuery : IRequest<OperationResult<UserProfile>>
{
    public Guid UserProfileId { get; set; }
}