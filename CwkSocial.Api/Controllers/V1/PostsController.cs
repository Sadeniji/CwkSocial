using AutoMapper;
using CwkSocial.Api.Contracts.Common;
using CwkSocial.Api.Contracts.Posts.Requests;
using CwkSocial.Api.Contracts.Posts.Responses;
using CwkSocial.Api.Extensions;
using CwkSocial.Api.Filters;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.Application.Posts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.BaseRoute)]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<IActionResult> GetAllPosts(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllPostsQuery(), cancellationToken);

            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(_mapper.Map<IEnumerable<PostResponse>>(result.Payload));
        }
        [HttpGet]
        [Route(ApiRoutes.Posts.IdRoute)]
        [ValidateGuid("id")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            var query = new GetPostByIdQuery() { PostId = Guid.Parse(id) };

            var response = await _mediator.Send(query, cancellationToken);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(_mapper.Map<PostResponse>(response.Payload));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreatePost([FromBody] CreatePost post, CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            
            var command = new CreatePostCommand
            {
                UserProfileId = userProfileId,
                TextContent = post.Text
            };

            var response = await _mediator.Send(command, cancellationToken);

            return response.IsError
                ? HandleErrorResponse(response.Errors)
                : CreatedAtAction(nameof(GetById), new { id = response.Payload.UserProfileId },
                    _mapper.Map<PostResponse>(response.Payload));
        }

        [HttpPatch]
        [Route(ApiRoutes.Posts.IdRoute)]
        [ValidateGuid("id")]
        [ValidateModel]
        public async Task<IActionResult> UpdatePostText([FromBody] UpdatePost updatePost, string id, CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();

            var command = new UpdatePostCommand
            {
                PostId = Guid.Parse(id),
                UserProfileId = userProfileId,
                NewText = updatePost.Text
            };

            var response = await _mediator.Send(command, cancellationToken);
            return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.Posts.IdRoute)]
        [ValidateGuid("id")]
        [ValidateGuid("profileId")]
        public async Task<IActionResult> DeletePost(string id, string profileId, CancellationToken cancellationToken)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            
            var command = new DeletePostCommand
            {
                PostId = Guid.Parse(id),
                UserProfileId = userProfileId
            };

            var response = await _mediator.Send(command, cancellationToken);
            
            return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Posts.PostComments)]
        [ValidateGuid("postId")]
        public async Task<IActionResult> GetCommentsByPostId(string postId, CancellationToken cancellationToken)
        {
            var query = new GetPostCommentsCommand
            {
                PostId = Guid.Parse(postId)
            };

            var response = await _mediator.Send(query, cancellationToken);

            return response.IsError 
                ? HandleErrorResponse(response.Errors) 
                : Ok(_mapper.Map<IEnumerable<PostCommentResponse>>(response.Payload));
        }

        [HttpPost]
        [Route(ApiRoutes.Posts.PostComments)]
        [ValidateGuid("postId")]
        [ValidateModel]
        public async Task<IActionResult> AddCommentToPost(string postId, [FromBody] CreatePostComment comment, CancellationToken cancellationToken)
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

                return BadRequest(apiError);
            }
            var command = new AddPostCommentCommand
            {
                PostId = Guid.Parse(postId),
                CommentText = comment.Text,
                UserProfileId = userProfileId
            };
            
            var response = await _mediator.Send(command, cancellationToken);

            return response.IsError
                ? HandleErrorResponse(response.Errors)
                : Ok(_mapper.Map<PostCommentResponse>(response.Payload));
        }

        [HttpGet]
        [Route(ApiRoutes.Posts.PostInteractions)]
        [ValidateGuid("postId")]
        public async Task<IActionResult> GetPostInteractions(string postId, CancellationToken cancellationToken)
        {
            var postIdInGuid = Guid.Parse(postId);

            var query = new GetPostInteractionsQuery
            {
                PostId = postIdInGuid
            };
            
            var response = await _mediator.Send(query, cancellationToken);
            
            return response.IsError 
                ? HandleErrorResponse(response.Errors) 
                : Ok(_mapper.Map<IEnumerable<PostInteractionResponse>>(response.Payload));
        }
        
        [HttpPost]
        [Route(ApiRoutes.Posts.PostInteractions)]
        [ValidateGuid("postId")]
        [ValidateModel]
        public async Task<IActionResult> AddPostInteraction(string postId, [FromBody] PostInteractionCreate interaction, CancellationToken cancellationToken)
        {
            var postIdInGuid = Guid.Parse(postId);
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            
            var command = new CreatePostInteractionCommand
            {
                PostId = postIdInGuid,
                UserProfileId = userProfileId,
                InteractionType = interaction.InteractionType
            };
            
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsError
                ? HandleErrorResponse(result.Errors)
                : Ok(_mapper.Map<PostInteractionResponse>(result.Payload));
        }
        
        [HttpDelete]
        [Route(ApiRoutes.Posts.InteractionById)]
        [ValidateGuid("postId", "interactionId")]
        public async Task<IActionResult> RemovePostInteraction(string postId, string interactionId, CancellationToken cancellationToken)
        {
            var postIdInGuid = Guid.Parse(postId);
            var interactionIdInGuid = Guid.Parse(interactionId);
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();
            
            var command = new DeletePostInteractionCommand
            {
                PostId = postIdInGuid,
                UserProfileId = userProfileId,
                InteractionId = interactionIdInGuid
            };
            
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsError
                ? HandleErrorResponse(result.Errors)
                : Ok(result.Payload);
        }
    }
}
