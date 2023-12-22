using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.Services;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class RegisterIdentityCommandHandler : IRequestHandler<RegisterIdentityCommand, OperationResult<string>>
{
    private readonly DataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IdentityService _identityService;

    public RegisterIdentityCommandHandler(DataContext dataContext, UserManager<IdentityUser> userManager, IdentityService identityService)
    {
        _dataContext = dataContext;
        _userManager = userManager;
        _identityService = identityService;
    }

    public async Task<OperationResult<string>> Handle(RegisterIdentityCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();

        try
        {
            var validateIdentityUserExist = await ValidateIdentityUserExistence(result, request.UserName);

            if (!validateIdentityUserExist)
            {
                return result;
            }
            
            // creating transaction
            await using var transaction = await _dataContext.Database.BeginTransactionAsync(cancellationToken);

            var identity = await CreateIdentityUserAsync(result, request, transaction, cancellationToken);

            if (identity == null)
            {
                return result;
            }

            var profile = await CreateUserProfileAsync(result, request, transaction, identity, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            result.Payload = GetJwtString(identity, profile);;
            return result;
        }
        catch (UserProfileNotValidException ex)
        {
            result.IsError = true;
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

    private async Task<bool> ValidateIdentityUserExistence(OperationResult<string> result, string userName)
    {
        var existingIdentity = await _userManager.FindByEmailAsync(userName);

        if (existingIdentity != null)
        {
            result.IsError = true;
            result.Errors.Add(new Error
            {
                Code = ErrorCode.IdentityUserAlreadyExists,
                Message = "Provided email address already exists. Cannot register new user"
            });

            return false;
        }
        return true;
    }

    private async Task<IdentityUser?> CreateIdentityUserAsync(OperationResult<string> result, RegisterIdentityCommand request, 
        IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        var identity = new IdentityUser
        {
            Email = request.UserName,
            UserName = request.UserName
        };
        
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
            return null;
        }
        
        return identity;
    }

    private async Task<UserProfile> CreateUserProfileAsync(OperationResult<string> result, RegisterIdentityCommand request,
        IDbContextTransaction transaction, IdentityUser identity, CancellationToken cancellationToken)
    {
        try
        {
            var profileInfo = BasicInfo.CreateBasicInfo(
                request.FirstName,
                request.LastName,
                request.UserName,
                request.Phone,
                request.DateOfBirth,
                request.CurrentCity);   

            var profile = UserProfile.CreateUserProfile(identity.Id, profileInfo);
            
            _dataContext.UserProfiles.Add(profile);
            await _dataContext.SaveChangesAsync(cancellationToken);
            return profile;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private string GetJwtString(IdentityUser identity, UserProfile profile)
    {
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, identity.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, identity.Email),
            new Claim("IdentityId", identity.Id),
            new Claim("UserProfileId", profile.UserProfileId.ToString())
        });

        var token = _identityService.CreateSecurityToken(claimsIdentity);
        return _identityService.WriteToken(token);
    }
}