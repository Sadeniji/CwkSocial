﻿using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Commands;

public class AddPostCommentCommand : IRequest<OperationResult<PostComment>>
{
    public Guid PostId { get; set; }
    public Guid UserProfileId { get; set; } 
    public string CommentText { get; set; }
}