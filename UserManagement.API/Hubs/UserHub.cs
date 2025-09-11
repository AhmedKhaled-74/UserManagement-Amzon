using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace UserManagement.API.Hubs
{
    [Authorize(Roles = "Admin")]
    public class UserHub : Hub
    {
    }
}
