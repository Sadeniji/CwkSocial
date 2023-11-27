using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.QueryHandlers;

public class GetPostCommentsCommandHandler : IRequestHandler<GetPostCommentsCommand, OperationResult<IEnumerable<PostComment>>>
{
    private readonly DataContext _dataContext;

    public GetPostCommentsCommandHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<IEnumerable<PostComment>>> Handle(GetPostCommentsCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<IEnumerable<PostComment>>();

        try
        {
            var post = await _dataContext.Posts.Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

            if (post is null)
                return result;
            
            result.Payload = post.Comments;
        }
        catch (Exception ex)
        {
            result.IsError = true;
            result.Errors.Add(new Error
            {
                Code = ErrorCode.UnknownError,
                Message = ex.Message
            });
        }
        return result;
    }
}