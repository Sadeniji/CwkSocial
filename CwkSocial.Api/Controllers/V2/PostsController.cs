using CwkSocial.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V2
{
    [ApiVersion("2.0")]

    [Route("api/v{version:apiVersion}/[controller]")]
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
                Text = "Hello Universe!"
            };

            return Ok(post);
        }
    }
}
