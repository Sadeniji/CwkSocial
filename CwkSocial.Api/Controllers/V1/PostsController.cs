﻿using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.BaseRoute)]
    [ApiController]
    public class PostsController : ControllerBase
    {
        [HttpGet]
        [Route(ApiRoutes.Posts.GetById)]
        public IActionResult GetById(Guid id)
        {
            return Ok();
        }
    }
}
