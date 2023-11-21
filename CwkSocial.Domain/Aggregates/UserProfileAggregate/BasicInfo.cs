using CwkSocial.Domain.Exceptions;
using CwkSocial.Domain.Validators.UserProfileValidator;

namespace CwkSocial.Domain.Aggregates.UserProfileAggregate;

public class BasicInfo
{
    private BasicInfo(){}
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }
    public string Phone{ get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string CurrentCity { get; private set; }

    /// <summary>
    /// Create a new BasicInfo instances
    /// </summary>
    /// <param name="firstName">First Name</param>
    /// <param name="lastName">Last Name</param>
    /// <param name="email">Email Address</param>
    /// <param name="phone">Phone</param>
    /// <param name="dateOfBirth">Date of Birth</param>
    /// <param name="currentCity">Current City</param>
    /// <returns><see cref="BasicInfo"/></returns>
    /// <exception cref="UserProfileNotValidException"></exception>
    public static BasicInfo CreateBasicInfo(string firstName,
        string lastName,
        string email,
        string phone,
        DateTime dateOfBirth,
        string currentCity)
    {
        var basicInfoValidator = new BasicInfoValidator();

        var basicInfoToValidate = new BasicInfo
        {
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = email,
            Phone = phone,
            CurrentCity = currentCity,
            DateOfBirth = dateOfBirth
        };

        var validationResult = basicInfoValidator.Validate(basicInfoToValidate);

        if (validationResult.IsValid)
            return basicInfoToValidate;

        var exception = new UserProfileNotValidException("The user profile is not valid");

        foreach (var error in validationResult.Errors)
        {
            exception.ValidationErrors.Add(error.ErrorMessage);
        }

        throw exception;
    }

}