﻿using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Commands;

public class UpdatePostCommand : IRequest<OperationResult<Post>>
{
    public Guid PostId { get; set; }
    public string NewText { get; set; }
    public Guid UserProfileId { get; set; }
}