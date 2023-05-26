using CwkSocial.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(Guid id)
        {
            var post = new Post
            {
                Id = id,
                Text = "Hello World!"
            };

            return Ok(post);
        }
    }
}
