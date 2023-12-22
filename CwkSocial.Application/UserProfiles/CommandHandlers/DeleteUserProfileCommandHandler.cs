using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.CommandHandlers;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, OperationResult<UserProfile>>
{
    private readonly DataContext _context;

    public DeleteUserProfileCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<UserProfile>> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<UserProfile>();
        
        var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserProfileId == request.UserProfileId, cancellationToken);
        
        if (userProfile is null)
        {
            result.IsError = true;
            result.Errors.Add(new Error
            {
                Code = ErrorCode.NotFound,
                Message = $"No UserProfile found for userProfileId - {request.UserProfileId}"
            });
            return result;
        }
        
        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync(cancellationToken);
        result.Payload = userProfile;
        return result;
    }
}