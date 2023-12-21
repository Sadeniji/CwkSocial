using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.Options;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class RegisterIdentityCommandHandler : IRequestHandler<RegisterIdentityCommand, OperationResult<string>>
{
    private readonly DataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public RegisterIdentityCommandHandler(DataContext dataContext, UserManager<IdentityUser> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _dataContext = dataContext;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<OperationResult<string>> Handle(RegisterIdentityCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();

        try
        {
            var existingIdentity = await _userManager.FindByEmailAsync(request.UserName);

            if (existingIdentity != null)
            {
                result.IsError = true;
                result.Errors.Add(new Error
                {
                    Code = ErrorCode.IdentityUserAlreadyExists,
                    Message = "Provided email address already exists. Cannot register new user"
                });

                return result;
            }

            var identity = new IdentityUser
            {
                Email = request.UserName,
                UserName = request.UserName
            };
            
            // creating transaction
            await using var transaction = await _dataContext.Database.BeginTransactionAsync(cancellationToken);
            
            var createdIdentity = await _userManager.CreateAsync(identity, request.Password);

            if (!createdIdentity.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                result.IsError = true;
                foreach (var identityError in createdIdentity.Errors)
                {
                    result.Errors.Add(new Error
                    {
                        Code = ErrorCode.IdentityCreationFailed,
                        Message = $"Unable to create new identity - {identityError.Description}"
                    });
                }
                return result;
            }
            
            var profileInfo = BasicInfo.CreateBasicInfo(
                request.FirstName,
                request.LastName,
                request.UserName,
                request.Phone,
                request.DateOfBirth,
                request.CurrentCity);

            var profile = UserProfile.CreateUserProfile(identity.Id, profileInfo);
            
            try
            {
                _dataContext.UserProfiles.Add(profile);
                await _dataContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SigningKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, identity.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, identity.Email),
                    new Claim("IdentityId", identity.Id),
                    new Claim("UserProfileId", profile.UserProfileId.ToString())
                }),
                Expires = DateTime.Now.AddHours(2),
                Audience = _jwtSettings.Audiences[0],
                Issuer = _jwtSettings.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            result.Payload = tokenHandler.WriteToken(token);
            return result;
        }
        catch (UserProfileNotValidException ex)
        {
            result.IsError = true;

            // foreach (var ve in ex.ValidationErrors)
            // {
            //     result.Errors.Add(new Error
            //     {
            //         Code = ErrorCode.ValidationError,
            //         Message = ve
            //     });
            // }
            ex.ValidationErrors.ForEach(e =>
            {
                result.Errors.Add(new Error
                {
                    Code = ErrorCode.ValidationError,
                    Message = e
                });
            });
        }
        catch (Exception ex)
        {
            result.IsError = true;
            result.Errors.Add(new Error
            {
                Code = ErrorCode.UnknownError,
                Message = ex.Message
            });
        }

        return result;
    }
}