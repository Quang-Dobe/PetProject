using Microsoft.AspNetCore.Mvc;
using PetProject.OrderManagement.Domain.Services;

namespace PetProject.OrderManagement.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) 
        { 
            _userService = userService;
        }
    }
}
