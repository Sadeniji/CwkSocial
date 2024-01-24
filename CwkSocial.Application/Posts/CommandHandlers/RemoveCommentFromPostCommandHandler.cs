using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class RemoveCommentFromPostCommandHandler : IRequestHandler<RemoveCommentFromPostCommand, OperationResult<bool>>
{
    private readonly DataContext _dataContext;
    private readonly OperationResult<bool> _result;

    public RemoveCommentFromPostCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
        _result = new OperationResult<bool>();
    }

    public async Task<OperationResult<bool>> Handle(RemoveCommentFromPostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _dataContext.Posts
                .Include(c => c.Comments)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

            if (post is null)
            {
                _result.AddError(ErrorCode.NotFound, PostErrorMessage.PostNotFound);
                return _result;
            }

            var comment = post.Comments.FirstOrDefault(c => c.CommentId == request.CommentId);
            
            if (comment is null)
            {
                _result.AddError(ErrorCode.NotFound, PostErrorMessage.PostCommentNotFound);
                return _result;
            }

            if (comment.UserProfileId != request.UserProfileId)
            {
                _result.AddError(ErrorCode.CommentRemovalNotAuthorized, PostErrorMessage.CommentRemovalNotAuthorized);
                return _result;
            }
            
            post.RemoveComment(comment);
            _dataContext.Posts.Update(post);
            await _dataContext.SaveChangesAsync(cancellationToken);

            _result.Payload = true;
            return _result;
        }
        catch (Exception ex)
        {
            _result.AddUnKnowError(ex.Message);
            return _result;
        }
    }
}