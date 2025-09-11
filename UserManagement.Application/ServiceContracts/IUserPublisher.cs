using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.ServiceContracts
{
    public interface IUserPublisher
    {
        Task PublishUserAdded(UserForAdminDTO user);
        Task PublishUserUpdated(UserForAdminDTO user);
    }
}
