﻿using System.ComponentModel.DataAnnotations;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class UpdatePost
{
    [Required]
    public string Text { get; set; }
}