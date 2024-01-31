using AutoMapper;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Dtos;
using CwkSocial.Application.Models;
using CwkSocial.DAL;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Identity.Queries;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, OperationResult<IdentityUserProfileDto>>
{
    private readonly DataContext _dataContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMapper _mapper;
    private OperationResult<IdentityUserProfileDto> _result = new();

    public GetCurrentUserQueryHandler(DataContext dataContext, UserManager<IdentityUser> userManager, IMapper mapper)
    {
        _dataContext = dataContext;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<OperationResult<IdentityUserProfileDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var identity = await _userManager.GetUserAsync(request.ClaimsPrincipal);

        if (identity is null)
        {
            _result.AddError(ErrorCode.NotFound, IdentityErrorMessage.NonExistenceIdentityUser);
            return _result;
        }

        var profile = await _dataContext.UserProfiles
                                        .FirstOrDefaultAsync(up => up.UserProfileId == request.UserProfileId, cancellationToken);

        _result.Payload = _mapper.Map<IdentityUserProfileDto>(profile);
        _result.Payload.UserName = identity.UserName;
        return _result;
    }
}