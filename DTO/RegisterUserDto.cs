﻿using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.DTO
{
    public class RegisterUserDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
