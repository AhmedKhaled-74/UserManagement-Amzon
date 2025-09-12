using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.ServiceContracts;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            var expireValue = DateTime.UtcNow
               .AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));
            var expireRefrechValue = DateTime.UtcNow
               .AddMinutes(Convert.ToDouble(_configuration["Jwt:Refresh_EXPIRATION_MINUTES"]));

            Claim[] claims = new Claim[] {
                new Claim( JwtRegisteredClaimNames.Sub, user.Id.ToString()), //required subject userid
                new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //required jwt unique id
                new Claim(JwtRegisteredClaimNames.Iat,
                             new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                             ClaimValueTypes.Integer64), //required issued at time of generation

                new Claim( ClaimTypes.NameIdentifier, user.Email!.ToString()), //optional unique name id msh bytkrr unique in table
                new Claim( ClaimTypes.Name, user.FullName!.ToString()), //optional name of user
                new Claim( ClaimTypes.Email, user.Email!.ToString()), //optional name of user       
                new Claim(ClaimTypes.Role, user.RoleName ?? string.Empty)
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expireValue,
                signingCredentials: signingCredentials
                );
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.WriteToken(jwtSecurityToken);
            return new AuthenticationResponse()
            {
                Token = token,
                Email = user.Email,
                PersonName = user.FullName,
                UserName = user.Email,
                Expiration = expireValue,
                RoleName = user.RoleName,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDate = expireRefrechValue
            };
        }

        public ClaimsPrincipal? GetJwtPrincipal(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null; // or throw a custom exception if you want
            }
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                ValidateLifetime = false,
            };
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            var principal = jwtTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken? securityToken);

            if (principal == null || securityToken is not JwtSecurityToken jwtSecurity
                || !jwtSecurity.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                  StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
        public AuthenticationResponse CreateAccessTokenOnly(ApplicationUser user, string existingRefreshToken, DateTime? existingRefreshExpiry)
        {
            var expireValue = DateTime.UtcNow
               .AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            Claim[] claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Email!.ToString()),
                new Claim(ClaimTypes.Name, user.FullName!.ToString()),
                new Claim(ClaimTypes.Email, user.Email!.ToString()),
                new Claim(ClaimTypes.Role, user.RoleName ?? string.Empty)
    };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expireValue,
                signingCredentials: signingCredentials
            );

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.WriteToken(jwtSecurityToken);

            return new AuthenticationResponse()
            {
                Token = token,
                Email = user.Email,
                PersonName = user.FullName,
                UserName = user.Email,
                Expiration = expireValue,
                RoleName = user.RoleName,
                RefreshToken = existingRefreshToken,
                RefreshTokenExpirationDate = existingRefreshExpiry
            };
        }

        //refresh token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
