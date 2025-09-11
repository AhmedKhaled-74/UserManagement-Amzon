using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Hubs;
using UserManagement.Application.DTOs.UserDTOs;

using UserManagement.Application.ServiceContracts;

namespace UserManagement.Infrastructure.Services
{
    public class SignalRUserPublisher : IUserPublisher
    {
        private readonly IHubContext<UserHub> _hubContext;

        public SignalRUserPublisher(IHubContext<UserHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task PublishUserAdded(UserForAdminDTO user)
            => _hubContext.Clients.All.SendAsync("UserAdded", user);

        public Task PublishUserUpdated(UserForAdminDTO user)
            => _hubContext.Clients.All.SendAsync("UserUpdated", user);

    }

}
