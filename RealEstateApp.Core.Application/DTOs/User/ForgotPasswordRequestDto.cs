﻿namespace RealEstateApp.Core.Application.DTOs.User
{
    public class ForgotPasswordRequestDto
    {
        public required string UserName { get; set; }
        public string? Origin { get; set; }
    }
}
