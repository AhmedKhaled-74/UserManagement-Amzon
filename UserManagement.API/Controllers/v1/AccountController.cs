using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.ServiceContracts;
using UserManagement.Application.Services;
using UserManagement.Domain.Entities.Identity;
using System.Net;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using UserManagement.Application.DTOs.Mappers.AdminMappers;
using UserManagement.Domain.Enums;

namespace UserManagement.API.Controllers.v1
{
    /// <summary>
    /// Provides endpoints for user account management, including registration, login, email confirmation,  password
    /// reset, and token generation.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AccountController : CustomHelperController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserPublisher _userPublisher;
        private readonly ILogService _logService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;
        /// <summary>
        /// constructor for account controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="userPublisher"></param> 
        /// <param name="signInManager"></param>
        /// <param name="configuration"></param>
        /// <param name="jwtService"></param>
        /// <param name="emailSender"></param>
        public AccountController(
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUserPublisher userPublisher,
            ILogService logService
            ,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IJwtService jwtService,
            IEmailSender emailSender
        )
        {
            _logger = logger;
            _userManager = userManager;
            _userPublisher = userPublisher;
            _roleManager = roleManager;
            _logService = logService;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtService = jwtService;
            _emailSender = emailSender;
        }

        // Add this method to your controller
        private async Task<string?> GetRegionFromIpAsync()
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
                    return null;

                // Using a free GeoIP service
                using var httpClient = new HttpClient();
                var response = await httpClient.GetFromJsonAsync<IpApiResponse>($"http://ip-api.com/json/{ipAddress}");

                return response?.Country;
            }
            catch
            {
                return null;
            }
        }

        // IP API response class
  

        /// <summary>
        /// Register new user → send confirmation email
        /// </summary>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> PostRegister(RegisterDTO registerDTO)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(registerDTO.RoleName);
                if (role == null)
                {
                    return BadRequest("Invalid role specified");
                }
                ApplicationUser? existingUser = _userManager.FindByEmailAsync(registerDTO.Email!).Result;
                if (existingUser != null && existingUser.EmailConfirmed)
                {
                    return Problem("Duplicated Email");
                }
                else if (existingUser != null && !existingUser.EmailConfirmed)
                {
                    await _userManager.DeleteAsync(existingUser);
                }
                ApplicationUser appUser = registerDTO.ToApplicationUser();
                appUser.Role = role;
                appUser.ActiveStatus = appUser.EmailConfirmed ? "Active" : "Suspend";

                var result = await _userManager.CreateAsync(appUser, registerDTO.Password);
                await _userPublisher.PublishUserAdded(appUser.ToUserAdminDTO());
                await _logService.LogActivityAsync(appUser.Id, "User Registered");
                if (!result.Succeeded)
                {
                    return Problem(string.Join("|", result.Errors.Select(e => e.Description)));
                }
                appUser.Region = registerDTO.Region ?? await GetRegionFromIpAsync() ?? "Egypt";

                // Set foreign keys for related entities
                appUser.Phones?.Where(p => p != null)
                    .ToList()
                    .ForEach(phone => phone!.UserId = appUser.Id);

                appUser.Addresses?.Where(p => p != null)
                    .ToList()
                    .ForEach(address => address!.UserId = appUser.Id);
                await _userManager.UpdateAsync(appUser);
                var roleResult = await _userManager.AddToRoleAsync(appUser, registerDTO.RoleName);
                if (!roleResult.Succeeded)
                {
                    return Problem(string.Join("|", roleResult.Errors.Select(e => e.Description)));
                }

                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                var confirmationLink = Url.Action(
                    nameof(ConfirmEmail),
                    "Account",
                    new { userId = appUser.Id, token },
                    Request.Scheme);

                // Send confirmation email
                await _emailSender.SendEmailAsync(
                    appUser.Email!,
                    "Confirm your email",
                    $"<p>Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.</p>");
                await _logService.LogActivityAsync(appUser.Id, "Confirmation Email Sent");

                return Ok(new
                {
                    message = "Registration successful! Please check your email to confirm your account."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in registration");
                await _logService.LogActivityAsync(null, $"Registration error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Email confirmation endpoint
        /// </summary>
        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Invalid user.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest("Email confirmation failed.");

            var authenticationResponse = _jwtService.CreateJwtToken(user);
            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpirationDate;
            user.ActiveStatus = "Active";

            await _userManager.UpdateAsync(user);
            await _userPublisher.PublishUserUpdated(user.ToUserAdminDTO());
            await _logService.LogActivityAsync(user.Id, "Email Confirmed");

            // redirect directly to Angular app
            var redirectUrl = $"{_configuration["Jwt:Audience"]}/account/confirm-email-success" +
                              $"?token={authenticationResponse.Token}" +
                              $"&refreshToken={authenticationResponse.RefreshToken}" +
                              $"&userName={Uri.EscapeDataString(user.UserName!)}";
            await _emailSender.SendEmailAsync(
                           user.Email!,
                           "Email Confirmation",
                           $"Email has been confirm successfully.");
            await _logService.LogActivityAsync(user.Id, "Confirmation Success Email Sent");
            return Redirect(redirectUrl);
        }

        /// <summary> /// 
        /// Check Email Exists 
        /// /// </summary> /// 
        /// <param name="email">email</param> 
        /// /// <returns>true or false</returns> 
        [HttpGet]
        [Route("register/check-email")]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && user.EmailConfirmed)
                {
                    return Ok(true);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckEmailExists");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Login
        /// </summary>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    loginDTO.Email!, loginDTO.Password!, loginDTO.IsPersistent, false);
                // Get client IP (supports IPv4 and IPv6)
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _logService.LogLoginActivityAsync((await _userManager.FindByEmailAsync(loginDTO.Email!))?.Id, result.Succeeded ? LoginAttempts.Success : LoginAttempts.Failed,ip);

                if (!result.Succeeded)
                    return Problem("Invalid email or password", statusCode: 401);

                var user = await _userManager.FindByEmailAsync(loginDTO.Email!);
                if (user == null) return Problem("Invalid user", statusCode: 401);
                if (user.ActiveStatus != "Active") return Problem("User is deactivated by admin",statusCode: 403);
                if (!user.EmailConfirmed) return Problem("Email is not confirmed",statusCode:401);

                var authenticationResponse = _jwtService.CreateJwtToken(user);
                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpirationDate;

                await _userManager.UpdateAsync(user);

                return Ok(authenticationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in login");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// logout from system
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("logout")]
        public async Task<ActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(new { message = "LogOut Done" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in logout");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// use to reset password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassRequest request)
        {
            if (request.Email == null)
                return BadRequest("email needed bad request.");
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.EmailConfirmed)
            {
                return BadRequest("Invalid request.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _logService.LogActivityAsync(user.Id, "Password Reset Token Generated");
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var resetLink = Url.Action(
                nameof(ConfirmResetPassword),
                "Account",
                new { userId = user.Id, token = encodedToken, request.Email },
                Request.Scheme
            );
            await _emailSender.SendEmailAsync(
                user.Email!,
                "Reset your password",
                $"<p>You can reset your password by clicking <a href='{resetLink}'>here</a>.</p>");
            await _logService.LogActivityAsync(user.Id, "Password Reset Email Sent");
            return Ok(new { message = "Password reset link has been sent to your email." });
        }
        /// <summary>
        /// Get Email confirmation
        /// </summary>
        [HttpGet]
        [Route("confirm-reset-password")]
        public async Task<IActionResult> ConfirmResetPassword(string userId, string token, string email)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Invalid user.");


            // redirect directly to Angular app
            var redirectUrl = $"{_configuration["Jwt:Audience"]}/account/reset-password" +
                              $"?token={token}" +
                              $"&email={email}"
                             ;

            return Redirect(redirectUrl);
        }
        /// <summary>            
        ///  accept pass reset confirmation        
        /// </summary>
        [HttpPost]
        [Route("confirm-reset-password")]
        public async Task<IActionResult> ConfirmResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email!);
            if (user == null) return BadRequest("Invalid user.");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDTO.Token!));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDTO.NewPassword);
            if (!result.Succeeded) return BadRequest("Password renew failed.");
            await _logService.LogActivityAsync(user.Id, "Password Reset Successfully");
            await _emailSender.SendEmailAsync(
                            user.Email!,
                            "Reset password",
                            $"Password has been reset successfully.");
            return Ok(new { message = "Password has been reset successfully." });
        }

        /// <summary>
        /// Generate New Token by refreshtoken
        /// </summary>
        /// <param name="token">exsist token</param>
        /// <returns>new auth instance</returns>
        [HttpPost]
        [Route("generate-new-token")]

        public async Task<IActionResult> GenerateNewToken(TokenModel token)
        {

            if (token == null)
            {
                return BadRequest("Invalid Client Request");

            }
            var principal = _jwtService.GetJwtPrincipal(token.Token);
            if (principal == null)
                return BadRequest("Invalid Jwt access token");

            var email = principal.FindFirstValue(ClaimTypes.Email);
            var role = principal.FindFirstValue(ClaimTypes.Role);
            if (email == null)
                return BadRequest("Invalid Jwt access token no email claim");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
                return BadRequest("Invalid Jwt access refreshtoken");
            if (user.ActiveStatus != "Active") return Problem("User is deactivated by admin", statusCode: 403);
            if (user.RoleName != role) return BadRequest("Invalid access role changed");

            var authRes = _jwtService.CreateAccessTokenOnly(user, user.RefreshToken!, user.RefreshTokenExpiration);

            return Ok(authRes);
        }
    }
}
