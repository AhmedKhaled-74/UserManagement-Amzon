using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetJwtPrincipal(string? token);
        AuthenticationResponse CreateAccessTokenOnly(ApplicationUser user, string existingRefreshToken, DateTime? existingRefreshExpiry);
    }
}
