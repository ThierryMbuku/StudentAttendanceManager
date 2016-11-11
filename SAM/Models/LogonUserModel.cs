using System;
using System.ComponentModel.DataAnnotations;
namespace SAM1.Models
{
    public class LogonUserModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string ChallengeResponse { get; set; }
    }
}