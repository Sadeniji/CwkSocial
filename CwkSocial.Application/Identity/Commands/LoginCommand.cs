using CwkSocial.Application.Identity.Dtos;
using CwkSocial.Application.Models;
using MediatR;

namespace CwkSocial.Application.Identity.Commands
{
    public class LoginCommand : IRequest<OperationResult<IdentityUserProfileDto>>
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
