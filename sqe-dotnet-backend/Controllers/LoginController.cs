using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SQE.Backend.DTOs;
using SQE.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace sqe_dotnet_backend.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IHttpContextAccessor _accessor;

        public UserController(IUserService userServiceAuthenticate, IHttpContextAccessor accessor)
        {
            _userService = userServiceAuthenticate;
            _accessor = accessor;
        }

        [AllowAnonymous]
        [HttpPost("login")] // api/user/login
        public IActionResult Authenticate([FromBody]UserInformation userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet("check")] // api/user/check
        public ActionResult<string> getCurrentUser()
        {
            return Ok(_accessor.HttpContext.User.Identity.Name);
        }

    }
}