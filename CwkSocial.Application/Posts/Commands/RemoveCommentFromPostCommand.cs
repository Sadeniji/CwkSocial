using CwkSocial.Application.Models;
using MediatR;

namespace CwkSocial.Application.Posts.Commands;

public record RemoveCommentFromPostCommand(Guid UserProfileId, Guid PostId, Guid CommentId) : IRequest<OperationResult<bool>>;