namespace CwkSocial.Api.Contracts.UserProfiles.Responses;

public record UserProfileResponse(Guid UserProfileId,  BasicInformation BasicInfo,
    DateTime DateCreated, DateTime LastModified);
// {
//     public Guid UserProfileId { get; set; }
//     public string IdentityId { get; set; }
//     public BasicInformation BasicInfo { get; set; }
//     public DateTime DateCreated { get; set; }
//     public DateTime LastModified { get; set; }
// }