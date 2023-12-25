using CwkSocial.Domain.Aggregates.PostAggregate;

namespace CwkSocial.Api.Contracts.Posts.Responses;

public class PostInteractionResponse
{
    public Guid InteractionId { get; set; }
    public string InteractionType { get; set; }
    public InteractionUser Author { get; set; }
}