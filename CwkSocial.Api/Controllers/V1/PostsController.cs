using AutoMapper;
using CwkSocial.Api.Contracts.Common;
using CwkSocial.Api.Contracts.Posts.Requests;
using CwkSocial.Api.Contracts.Posts.Responses;
using CwkSocial.Api.Filters;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.Application.Posts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.BaseRoute)]
    [ApiController]
    public class PostsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PostsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await _mediator.Send(new GetAllPostsQuery());

            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(_mapper.Map<IEnumerable<PostResponse>>(result.Payload));
        }
        [HttpGet]
        [Route(ApiRoutes.Posts.IdRoute)]
        [ValidateGuid("id")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new GetPostByIdQuery() { PostId = Guid.Parse(id) };

            var response = await _mediator.Send(query);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(_mapper.Map<PostResponse>(response.Payload));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreatePost([FromBody] CreatePost post)
        {
            var command = new CreatePostCommand
            {
                UserProfileId = post.UserProfileId,
                TextContent = post.TextContent
            };

            var response = await _mediator.Send(command);

            return response.IsError
                ? HandleErrorResponse(response.Errors)
                : CreatedAtAction(nameof(GetById), new { id = response.Payload.UserProfileId },
                    _mapper.Map<PostResponse>(response.Payload));
        }

        [HttpPatch]
        [Route(ApiRoutes.Posts.IdRoute)]
        [ValidateGuid("id")]
        [ValidateModel]
        public async Task<IActionResult> UpdatePostText([FromBody] UpdatePost updatePost, string id)
        {
            var command = new UpdatePostCommand
            {
                PostId = Guid.Parse(id),
                UserProfileId = Guid.Parse(updatePost.UserProfileId),
                NewText = updatePost.Text
            };

            var response = await _mediator.Send(command);

            return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.Posts.IdRoute)]
        [ValidateGuid("id")]
        [ValidateGuid("profileId")]
        public async Task<IActionResult> DeletePost(string id, string profileId)
        {
            var command = new DeletePostCommand
            {
                PostId = Guid.Parse(id),
                UserProfileId = Guid.Parse(profileId)
            };

            var response = await _mediator.Send(command);
            
            return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Posts.PostComments)]
        [ValidateGuid("postId")]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            var query = new GetPostCommentsCommand
            {
                PostId = Guid.Parse(postId)
            };

            var response = await _mediator.Send(query);

            return response.IsError 
                ? HandleErrorResponse(response.Errors) 
                : Ok(_mapper.Map<IEnumerable<PostCommentResponse>>(response.Payload));
        }

        [HttpPost]
        [Route(ApiRoutes.Posts.PostComments)]
        [ValidateGuid("postId")]
        [ValidateModel]
        public async Task<IActionResult> AddCommentToPost(string postId, [FromBody] CreatePostComment comment)
        {
            var isValidUser = Guid.TryParse(comment.UserProfileId, out var userProfileId);

            if (!isValidUser)
            {
                var apiError = new ErrorResponse
                {
                    StatusCode = 400,
                    StatusPhrase = "Bad Request",
                    TimeStamp = DateTime.Now,
                    Errors = { "Provided user profile Id is invalid" }
                };
            }
            var command = new AddPostCommentCommand
            {
                PostId = Guid.Parse(postId),
                CommentText = comment.Text,
                UserProfileId = userProfileId
            };
            
            var response = await _mediator.Send(command);

            return response.IsError
                ? HandleErrorResponse(response.Errors)
                : Ok(_mapper.Map<PostCommentResponse>(response.Payload));
        }
        
    }
}
