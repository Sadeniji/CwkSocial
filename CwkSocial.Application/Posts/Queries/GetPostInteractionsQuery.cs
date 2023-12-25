using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregates.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Queries;

public class GetPostInteractionsQuery : IRequest<OperationResult<List<PostInteraction>>>
{
    public Guid PostId { get; set; }
}