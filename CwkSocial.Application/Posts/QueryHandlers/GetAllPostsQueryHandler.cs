using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.QueryHandlers;

public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, OperationResult<IEnumerable<Post>>>
{
    private readonly DataContext _dataContext;

    public GetAllPostsQueryHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<OperationResult<IEnumerable<Post>>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<IEnumerable<Post>>();
        try
        {
            result.Payload = await _dataContext.Posts.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            result.AddError(ErrorCode.UnknownError, ex.Message);
        }
        return result;
    }
}