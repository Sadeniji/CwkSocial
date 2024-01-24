using CwkSocial.Application.Models;
using MediatR;

namespace CwkSocial.Application.Posts.Commands;

public record UpdatePostCommentCommand(
    Guid PostId, 
    Guid CommentId, 
    Guid UserProfileId,
    string UpdatedText) : IRequest<OperationResult<bool>>;