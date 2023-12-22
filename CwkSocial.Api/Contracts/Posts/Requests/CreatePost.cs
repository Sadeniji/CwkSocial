using System.ComponentModel.DataAnnotations;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class CreatePost
{
    [Required]
    [StringLength(1000)]
    public string Text { get; set; }
}