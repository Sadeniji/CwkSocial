﻿namespace CwkSocial.Api.Contracts.Identity;

public class AuthenticationResult
{
    public string UserName { get; set; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string EmailAddress { get; init; }
    public string Phone { get; init; }
    public  DateTime DateOfBirth { get; init; }
    public string CurrentCity { get; init; }
    public string Token { get; set; }
}