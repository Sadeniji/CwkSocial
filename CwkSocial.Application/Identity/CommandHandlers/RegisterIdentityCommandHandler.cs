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
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class RegisterIdentityCommandHandler : IRequestHandler<RegisterIdentityCommand, OperationResult<IdentityUserProfileDto>>
{
    private readonly DataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IdentityService _identityService;
    private OperationResult<IdentityUserProfileDto> _result = new OperationResult<IdentityUserProfileDto>();
    private readonly IMapper _mapper;

    public RegisterIdentityCommandHandler(DataContext dataContext, UserManager<IdentityUser> userManager, 
                                          IdentityService identityService, IMapper mapper)
    {
        _dataContext = dataContext;
        _userManager = userManager;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<OperationResult<IdentityUserProfileDto>> Handle(RegisterIdentityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validateIdentityUserExist = await ValidateIdentityUserExistence(request.UserName);

            if (!validateIdentityUserExist)
            {
                return _result;
            }
            
            // creating transaction
            await using var transaction = await _dataContext.Database.BeginTransactionAsync(cancellationToken);

            var identity = await CreateIdentityUserAsync(request, transaction, cancellationToken);

            if (identity == null)
            {
                return _result;
            }

            var profile = await CreateUserProfileAsync(request, transaction, identity, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _result.Payload = _mapper.Map<IdentityUserProfileDto>(profile);
            _result.Payload.Token = GetJwtString(identity, profile);
            _result.Payload.UserName = identity.UserName;
            return _result;
        }
        catch (UserProfileNotValidException ex)
        {
            ex.ValidationErrors.ForEach(e => _result.AddError(ErrorCode.ValidationError, e));
        }
        catch (Exception ex)
        {
            _result.AddError(ErrorCode.UnknownError, ex.Message);
        }

        return _result;
    }

    private async Task<bool> ValidateIdentityUserExistence(string userName)
    {
        var existingIdentity = await _userManager.FindByEmailAsync(userName);

        if (existingIdentity != null)
        {
            _result.AddError(ErrorCode.IdentityUserAlreadyExists, IdentityErrorMessage.IdentityUserAlreadyExist);
            return false;
        }
        return true;
    }

    private async Task<IdentityUser?> CreateIdentityUserAsync(RegisterIdentityCommand request, 
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
            
            foreach (var identityError in createdIdentity.Errors)
            {
                _result.AddError(ErrorCode.IdentityCreationFailed, identityError.Description);
            }
            return null;
        }
        return identity;
    }

    private async Task<UserProfile> CreateUserProfileAsync(RegisterIdentityCommand request,
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