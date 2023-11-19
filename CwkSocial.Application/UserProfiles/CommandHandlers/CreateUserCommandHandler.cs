using AutoMapper;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DAL;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.CommandHandlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult<UserProfile?>>
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OperationResult<UserProfile?>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<UserProfile>();

        var basicInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
            request.Phone, request.DateOfBirth, request.CurrentCity);

        var userProfile = UserProfile.CreateUserProfile(Guid.NewGuid().ToString(), basicInfo);
        
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync(cancellationToken);

        result.Payload = userProfile;
        return result;
    }
}