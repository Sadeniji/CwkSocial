using System.ComponentModel.DataAnnotations;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class UpdatePostComment
{
    [Required]
    public string Text { get; set; }
}