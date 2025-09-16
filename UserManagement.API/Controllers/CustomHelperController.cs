using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Presentation.Controllers
{
    /// <summary>
    /// Provides helper endpoints for custom operations within the Presentation.
    /// </summary>
    /// <remarks>This controller serves as a base for defining custom helper actions that can be accessed via
    /// the Presentation. It is decorated with versioning and routing attributes to support Presentation versioning and consistent
    /// endpoint structure.</remarks>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class CustomHelperController : ControllerBase
    {
    }
}