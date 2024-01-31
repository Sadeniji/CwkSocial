using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Identity.Dtos;
using CwkSocial.Application.Models;
using CwkSocial.Application.Services;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Identity.CommandHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<IdentityUserProfileDto>>
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityService _identityService;
        private readonly OperationResult<IdentityUserProfileDto> _result;
        private readonly IMapper _mapper;

        public LoginCommandHandler(DataContext dataContext, UserManager<IdentityUser> userManager, 
                                    IdentityService identityService, IMapper mapper)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _identityService = identityService;
            _mapper = mapper;
            _result = new OperationResult<IdentityUserProfileDto>();
        }

        public async Task<OperationResult<IdentityUserProfileDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var identityUser = await ValidateAndGetIdentityUserAsync(request);

                if (identityUser is null)
                {
                    return _result;
                }
                var userProfile = await _dataContext.UserProfiles.FirstOrDefaultAsync(up => up.IdentityId == identityUser.Id, cancellationToken);
                if (userProfile is null)
                {
                    _result.AddError(ErrorCode.InExistenceUserProfile,  $"Login failed. User profile does not exist - {request.UserName}.");
                    return _result;
                }

                _result.Payload = _mapper.Map<IdentityUserProfileDto>(userProfile);
                _result.Payload.Token = GetJwtString(identityUser, userProfile);
                _result.Payload.UserName = identityUser.UserName;
                return _result;
            } 
            catch (Exception ex)
            {
                _result.AddError(ErrorCode.UnknownError, ex.Message);
                return _result;
            }
        }

        private string GetJwtString(IdentityUser identityUser, UserProfile userProfile)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                new Claim("IdentityId", identityUser.Id),
                new Claim("UserProfileId", userProfile.UserProfileId.ToString())
            });

            var token = _identityService.CreateSecurityToken(claimsIdentity);
            return _identityService.WriteToken(token);
        }

        private async Task<IdentityUser?> ValidateAndGetIdentityUserAsync(LoginCommand request)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.UserName);

            if (identityUser == null)
            {
                _result.AddError(ErrorCode.IdentityUserDoesNotExist, IdentityErrorMessage.NonExistenceIdentityUser);
                return null;
            }

            var isAValidPassword = await _userManager.CheckPasswordAsync(identityUser, request.Password);

            if (!isAValidPassword)
            {
                _result.AddError(ErrorCode.IncorrectPassword, IdentityErrorMessage.InCorrectPassword);
                return null;
            }

            return identityUser;
        }
    }
}
