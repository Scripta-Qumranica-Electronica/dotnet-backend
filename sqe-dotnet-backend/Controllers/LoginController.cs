using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SQE.Backend.DTOs;
using SQE.Backend.Services;
using SQE.Backend.DataAccess;
using System.Threading.Tasks;

namespace sqe_dotnet_backend.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userServiceAuthenticate)
        {
            _userService = userServiceAuthenticate;
        }

        [AllowAnonymous]
        [HttpPost("login")] // api/user/login
        public async Task<IActionResult> AuthenticateAsync([FromBody]UserInformation userParam)
        {

            UserInformation user = await _userService.AuthenticateAsync(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet("check")] // api/user/check
        public ActionResult<UserInformation> getCurrentUser()
        {
            UserInformation user = _userService.getCurrentUser();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet("userId")] // api/user/check
        public ActionResult<string> getCurrentUserId()
        {
            return Ok(_userService.getCurrentUserId());
        }

    }
}