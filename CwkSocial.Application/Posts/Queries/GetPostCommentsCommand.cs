using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Queries;

public class GetPostCommentsCommand : IRequest<OperationResult<IEnumerable<PostComment>>>
{
    public Guid PostId { get; set; }
}