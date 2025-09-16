using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace UserManagement.Presentation.Hubs
{
    [Authorize(Roles = "Admin")]
    public class UserHub : Hub
    {
    }
}
