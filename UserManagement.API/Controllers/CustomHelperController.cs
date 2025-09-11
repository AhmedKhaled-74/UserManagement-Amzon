using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    /// <summary>
    /// Provides helper endpoints for custom operations within the API.
    /// </summary>
    /// <remarks>This controller serves as a base for defining custom helper actions that can be accessed via
    /// the API. It is decorated with versioning and routing attributes to support API versioning and consistent
    /// endpoint structure.</remarks>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class CustomHelperController : ControllerBase
    {
    }
}