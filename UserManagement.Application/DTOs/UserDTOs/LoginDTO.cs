using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.DTOs.UserDTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }


        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Invalid Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[_@*-])[a-zA-Z0-9_@*-]{10,21}$",
            ErrorMessage = "Invalid Password")]
        public string? Password { get; set; }


        public bool IsPersistent {  get; set; } = false;
    }
}
