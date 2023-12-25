using System.ComponentModel.DataAnnotations;
using CwkSocial.Domain.Aggregates.PostAggregate;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class PostInteractionCreate
{
    [Required]
    public InteractionType InteractionType { get; set; }
}