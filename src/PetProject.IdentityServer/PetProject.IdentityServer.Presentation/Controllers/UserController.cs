using Microsoft.AspNetCore.Mvc;
using PetProject.IdentityServer.Domain.DTOs.User.Request;
using PetProject.IdentityServer.Domain.Services;

namespace PetProject.IdentityServer.Presentation.Controllers
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

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDto userInformation)
        {
            try
            {
                await _userService.CreateUser(userInformation);

                return Ok();
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch 
            {
                return StatusCode(500, "Internal Error");
            }
        }

        [Route("Edit")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UserDto userInformation)
        {
            try
            {
                await _userService.UpdateUser(userInformation);

                return Ok();
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Internal Error");
            }
        }

        [Route("Active/{id}")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromRoute] Guid id, [FromBody] bool status)
        {
            try
            {
                await _userService.ChangeUserStatus(id, status);

                return Ok();
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Internal Error");
            }
        }
    }
}
