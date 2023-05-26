namespace CwkSocial.Domain.Models;

public class Post
{
    public Guid Id { get; set; }
    public string Text { get; set; }   = string.Empty;
}