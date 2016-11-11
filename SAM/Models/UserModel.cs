using System;
using System.ComponentModel.DataAnnotations;

namespace SAM1.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [StringLength(20, MinimumLength=5)]
        [Required]
        public string FirstName { get; set; }
        [StringLength(20, MinimumLength = 5)]
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CellPhone { get; set; }
        [StringLength(10, MinimumLength = 9)]
        [Required]
        public string Username { get; set; }
        public bool IsAdmin { get; set; }

        [Required]
        public AddressModel Address { get; set; }

        public int AdminUserId { get; set; }
    }

    public class AddressModel
    {
        [Required]
        public string ComplexNumber { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Suburb { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PostalCode { get; set; }
    }
}