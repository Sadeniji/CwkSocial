using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.Options;
using CwkSocial.Application.Services;
using CwkSocial.DAL;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
                var identityUser = await _userManager.FindByEmailAsync(request.UserName);

                if (identityUser == null)
                {
                    result.IsError = true;
                    result.Errors.Add(new Error
                    {
                        Code = ErrorCode.IdentityUserDoesNotExist,
                        Message = $"Login failed. Provided username is incorrect - {request.UserName}."
                    });

                    return result;
                }

                var isAValidPassword = await _userManager.CheckPasswordAsync(identityUser, request.Password);

                if (!isAValidPassword)
                {
                    result.IsError = true;
                    result.Errors.Add(new Error
                    {
                        Code = ErrorCode.IncorrectPassword,
                        Message = "Login failed. Provided password is incorrect."
                    });

                    return result;
                }

                var userProfile = await _dataContext.UserProfiles
                    .FirstOrDefaultAsync(up => up.IdentityId == identityUser.Id, cancellationToken);

                var claimsIdentity = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                    new Claim("IdentityId", identityUser.Id),
                    new Claim("UserProfileId", userProfile.UserProfileId.ToString())
                });

                var token = _identityService.CreateSecurityToken(claimsIdentity);
                result.Payload = _identityService.WriteToken(token);
                return result;
                // var tokenHandler = new JwtSecurityTokenHandler();
                // var key = Encoding.ASCII.GetBytes(_jwtSettings.SigningKey);
                // var tokenDescriptor = new SecurityTokenDescriptor
                // {
                //     Subject = new ClaimsIdentity(new Claim[]
                //     {
                //         new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //         new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                //         new Claim("IdentityId", identityUser.Id),
                //         new Claim("UserProfileId", userProfile.UserProfileId.ToString())
                //     }),
                //     Expires = DateTime.Now.AddHours(2),
                //     Audience = _jwtSettings.Audiences[0],
                //     Issuer = _jwtSettings.Issuer,
                //     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                //         SecurityAlgorithms.HmacSha256Signature)
                // };
                //
                // var token = tokenHandler.CreateToken(tokenDescriptor);
                // result.Payload = tokenHandler.WriteToken(token);
                // return result;
            } 
            catch (Exception ex)
            {
                result.IsError = true;
                result.Errors.Add(new Error
                {
                    Code = ErrorCode.UnknownError,
                    Message = ex.Message
                });
                return result;
            }
        }
    }
}
