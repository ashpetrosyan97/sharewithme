using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SWM.Application.Users
{
    public class ResetPasswordDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
    }
}
