using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.Mappers.AdminMappers;
using UserManagement.Application.DTOs.Mappers.UserMappers;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.ServiceContracts;

namespace UserManagement.API.Controllers.v1
{
    /// <summary>
    /// Provides endpoints for managing user-related data, including personal information, addresses, and phone numbers.
    /// </summary>
    /// <remarks>This controller handles operations such as retrieving, updating, and deleting user data, as
    /// well as managing default addresses and phone numbers. It requires authorization and supports API version
    /// 1.0.</remarks>

    [Authorize]
    [ApiVersion("1.0")]
    public class UsersController : CustomHelperController
    {
        private readonly IUserService _userService;
       
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userService"></param>

        /// <param name="logger"></param>
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
        
            _logger = logger;
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound($"User with email {email} not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update user personal information
        /// </summary>
        [HttpPut("{userId}/personal-info")]
        public async Task<IActionResult> UpdatePersonalInfo(
            Guid userId,
            [FromBody] UpdatePersonalInfoRequest request)
        {
            try
            {
               var user = await _userService.UpdatePersonalInfoAsync(
                    userId,
                    request);
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found or update failed");
                }


                return Ok(new { message = "Personal information updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating personal info for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
        ///address endpoints

        /// <summary>
        /// get user addresses
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        [HttpGet("{userId}/addresses")]
        public async Task<IActionResult> GetAddresses(Guid userId)
        {
            try
            {
                var addresses = await _userService.GetUserAddressesAsync(userId);
                if (addresses == null || !addresses.Any())
                {
                    return NotFound("No addresses found for the user");
                }
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving addresses for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Add address to user
        /// </summary>
        [HttpPost("{userId}/addresses")]
        public async Task<IActionResult> AddAddress(Guid userId, [FromBody] AddressDTO address)
        {
            try
            {
                var GetAddresses = await _userService.GetUserAddressesAsync(userId);
                if (GetAddresses == null || !GetAddresses.Any())
                {
                    address.IsDefault = true;
                }
                await _userService.AddAddressAsync(userId, address);
                return Ok(new { message = "Address added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding address for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update user address
        /// </summary>
        [HttpPut("addresses/{addressId}")]
        public async Task<IActionResult> UpdateAddress(Guid addressId, [FromBody] AddressDTO address)
        {
            try
            {
                await _userService.UpdateAddressAsync(addressId, address);
                return Ok(new { message = "Address updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address: {AddressId}", addressId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete user address
        /// </summary>
        [HttpDelete("{userId}/addresses/{addressId}")]
        public async Task<IActionResult> DeleteAddress(Guid userId, Guid addressId)
        {
            try
            {
                await _userService.DeleteAddressAsync(userId, addressId);
                return Ok(new { message = "Address deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address: {AddressId} for user: {UserId}", addressId, userId);
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// set a user address as default
        /// </summary>
        [HttpPatch("{userId}/addresses/{addressId}/default")]
        public async Task<IActionResult> SetDefaultAddress(Guid userId, Guid addressId)
        {
            if (userId == Guid.Empty || addressId == Guid.Empty)
            {
                return BadRequest("Invalid userId or addressId");
            }
            try
            {

                var addresses = await _userService.GetUserAddressesAsync(userId);
                if (addresses == null || !addresses.Any())
                {
                    return NotFound("No addresses found for the user");
                }
                var address = addresses.FirstOrDefault(a => a.AddressId == addressId);
                await _userService.SetAddressDefaultAsync(userId, addressId);
                return Ok(new { message = "address set to default success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error set default address: {AddressId} for user: {UserId}", addressId, userId);
                return StatusCode(500, "Internal server error");
            }
        }
        ///phone endpoints
        /// <summary>
        /// get user phones
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        [HttpGet("{userId}/phones")]
        public async Task<IActionResult> GetPhones(Guid userId)
        {
            try
            {
                var phones = await _userService.GetUserPhonesAsync(userId);
                if (phones == null || !phones.Any())
                {
                    return NotFound(new { message = "No phones found for the user" });
                }
                return Ok(phones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving phones for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Add phone to user
        /// </summary>
        [HttpPost("{userId}/phones")]
        public async Task<IActionResult> AddPhone(Guid userId, [FromBody] PhoneDTO phone)
        {
            try
            {
                var GetPhones = await _userService.GetUserPhonesAsync(userId);
                if (GetPhones == null || !GetPhones.Any())
                {
                    phone.IsDefault = true;
                }
                await _userService.AddPhoneAsync(userId, phone);
                return Ok(new { message = "Phone added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding phone for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update user phone
        /// </summary>
        [HttpPut("{userId}/phones")]
        public async Task<IActionResult> UpdatePhone(Guid userId, [FromBody] PhoneDTO phone)
        {
            try
            {
                await _userService.UpdatePhoneAsync(userId, phone);
                return Ok(new { message = "Phone updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating phone for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete user phone
        /// </summary>
        [HttpDelete("{userId}/phones/{phoneId}")]
        public async Task<IActionResult> DeletePhone(Guid userId, Guid phoneId)
        {
            try
            {
                await _userService.DeletePhoneAsync(userId, phoneId);
                return Ok(new { message = "Phone deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting phone: {PhoneId} for user: {UserId}", phoneId, userId);
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// set a user phone as default
        /// </summary>
        [HttpPatch("{userId}/phones/{phoneId}/default")]
        public async Task<IActionResult> SetDefaultPhone(Guid userId, Guid phoneId)
        {
            if (userId == Guid.Empty || phoneId == Guid.Empty)
            {
                return BadRequest("Invalid userId or phoneId");
            }
            try
            {

                var phones = await _userService.GetUserPhonesAsync(userId);
                if (phones == null || !phones.Any())
                {
                    return NotFound("No phones found for the user");
                }
                var phone = phones.FirstOrDefault(a => a.PhoneId == phoneId);
                await _userService.SetPhoneDefaultAsync(userId, phoneId);
                return Ok(new { message = "phone set to default success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error set default phone: {PhoneId} for user: {UserId}", phoneId, userId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}