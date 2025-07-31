﻿namespace RealEstateApp.Core.Application.DTOs.User
{
    public class UserResponseDto
    {
        public string? Message { get; set; }
        public bool HasError { get; set; }
        public required List<string> Errors { get; set; }
    }
}
