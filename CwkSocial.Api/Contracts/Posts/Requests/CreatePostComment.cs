using System.ComponentModel.DataAnnotations;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class CreatePostComment
{
    [Required]
    public string Text { get; set; }
}