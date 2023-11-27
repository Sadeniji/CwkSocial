using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, OperationResult<Post>>
{
    private readonly DataContext _dataContext;

    public DeletePostCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<Post>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Post>();

        try
        {
            var post = await _dataContext.Posts.FirstOrDefaultAsync(
                p => p.PostId == request.PostId && p.UserProfileId == request.UserProfileId, cancellationToken);
            
            if (post is null)
            {
                result.IsError = true;
                result.Errors.Add(new Error
                {
                    Code = ErrorCode.NotFound,
                    Message = $"No Post found for PostId - {request.PostId} with UserProfileId - {request.UserProfileId}"
                });
                return result;
            }
            
            _dataContext.Posts.Remove(post);
            await _dataContext.SaveChangesAsync(cancellationToken);
            result.Payload = post;
            return result;
        }
        catch (Exception ex)
        {
            result.IsError = true;
            result.Errors.Add(new Error
            {
                Code = ErrorCode.UnknownError,
                Message = ex.Message
            });
            return result;
        }
    }
}