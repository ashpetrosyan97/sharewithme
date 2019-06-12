using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SWM.Application.Users
{
    public class UpdateUserDto:EntityDto<long>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_]{6,}$", ErrorMessage = "Invalid username")]
        [Required]
        public string Username { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_]{6,}\@[a-zA-Z]{2,}\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address")]
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public int AccountId { get; set; }
    }
}
