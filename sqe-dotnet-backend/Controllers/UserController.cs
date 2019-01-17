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
        public async Task<IActionResult> AuthenticateAsync([FromBody]LoginRequest userParam)
        {
            LoginResponse user = await _userService.AuthenticateAsync(userParam.username, userParam.password);
      
            if (user == null)
                return Unauthorized(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpGet("check")] // api/user/check
        public ActionResult<LoginResponse> GetCurrentUser()
        {
            LoginResponse user = _userService.GetCurrentUser();

            return Ok(user);
        }
    }
}