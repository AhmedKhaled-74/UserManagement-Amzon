using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.DTOs.UserDTOs
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Full name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string? FullName { get; set; }


        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Remote("CheckEmailExists", "Account", ErrorMessage = "Email already exists.")]
        public string? Email { get; set; }


        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[_@*-])[a-zA-Z0-9_@*-]{10,21}$",
            ErrorMessage = "Password must contain at least one character from each category (letters, numbers, and special characters _, @, -, *)")]
        public string Password { get; set; } = null!;


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }


        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }


        [StringLength(50, ErrorMessage = "Region cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Region can only contain letters and spaces.")]
        public string? Region { get; set; }


        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        [RegularExpression(@"^\+?[0-9\s]+$", ErrorMessage = "Phone number can only contain numbers and spaces, and may start with a +.")]
        public PhoneDTO? Phone { get; set; }
        public AddressDTO? Address { get; set; }

        public string RoleName { get; set; } = "User";
        public ApplicationUser ToApplicationUser()
        {
            var appUser = new ApplicationUser
            {
                UserName = Email,
                Email = Email,
                FullName = FullName ?? "",
                DateOfBirth = DateOfBirth,
                Region = Region,
            };

            if (Phone != null)
            {
                var phoneEntity = Phone.ToUserPhone();
                phoneEntity.IsDefault = true;  
                appUser.Phones.Add(phoneEntity);
            }

            if (Address != null)
            {
                var addressEntity = Address.ToUserAddress();
                addressEntity.IsDefault = true;  
                appUser.Addresses.Add(addressEntity);
            }

            return appUser;
        }


    }
}
