﻿using CwkSocial.Application.Identity.Dtos;
using CwkSocial.Application.Models;
using MediatR;

namespace CwkSocial.Application.Identity.Commands;

public class RegisterIdentityCommand : IRequest<OperationResult<IdentityUserProfileDto>>
{
    public string UserName { get; set; }
    
    public string Password { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public DateTime DateOfBirth { get; set; }

    public string Phone { get; set; }
    public string CurrentCity { get; set; }
}