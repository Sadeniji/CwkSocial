using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.Services;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Identity.CommandHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<string>>
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityService _identityService;

        public LoginCommandHandler(DataContext dataContext, UserManager<IdentityUser> userManager, IdentityService identityService)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _identityService = identityService;
        }

        public async Task<OperationResult<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<string>();

            try
            {
                var identityUser = await ValidateAndGetIdentityUserAsync(result, request);

                if (identityUser is null)
                {
                    return result;
                }
                var userProfile = await _dataContext.UserProfiles.FirstOrDefaultAsync(up => up.IdentityId == identityUser.Id, cancellationToken);
                if (userProfile is null)
                {
                    result.AddError(ErrorCode.InExistenceUserProfile,  $"Login failed. User profile does not exist - {request.UserName}.");
                    return result;
                }
                result.Payload = GetJwtString(identityUser, userProfile);
                return result;
            } 
            catch (Exception ex)
            {
                result.AddError(ErrorCode.UnknownError, ex.Message);
                return result;
            }
        }

        private string GetJwtString(IdentityUser identityUser, UserProfile userProfile)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                new Claim("IdentityId", identityUser.Id),
                new Claim("UserProfileId", userProfile.UserProfileId.ToString())
            });

            var token = _identityService.CreateSecurityToken(claimsIdentity);
            return _identityService.WriteToken(token);
        }

        private async Task<IdentityUser?> ValidateAndGetIdentityUserAsync(OperationResult<string> result, 
            LoginCommand request)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.UserName);

            if (identityUser == null)
            {
                result.AddError(ErrorCode.IdentityUserDoesNotExist, IdentityErrorMessage.NonExistenceIdentityUser);
                return null;
            }

            var isAValidPassword = await _userManager.CheckPasswordAsync(identityUser, request.Password);

            if (!isAValidPassword)
            {
                result.AddError(ErrorCode.IncorrectPassword, IdentityErrorMessage.InCorrectPassword);
                return null;
            }

            return identityUser;
        }
    }
}
