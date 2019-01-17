using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SQE.Backend.DataAccess;
using SQE.Backend.DTOs;
using SQE.Backend.Helpers;


namespace SQE.Backend.Services
{
    public interface IUserService
    {
        Task<LoginResponse> AuthenticateAsync(string username, string password);
        LoginResponse GetCurrentUser();
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private IUserRepository _repo;
        private IHttpContextAccessor _accessor;


        public UserService(IOptions<AppSettings> appSettings, IUserRepository userRepository, IHttpContextAccessor accessor)
        {
            _repo = userRepository;
            _appSettings = appSettings.Value;
            _accessor = accessor;
        }


        public async Task<LoginResponse> AuthenticateAsync(string username, string password)
        {
            var result = await _repo.GetUserByPassword(username, password);

            if (result == null)
                return null;

            LoginResponse loginResponse = new LoginResponse { username = result.Username, userId = result.UserId };
            loginResponse.token = BuildUserToken(loginResponse.username, loginResponse.userId).ToString();

            return loginResponse;
        }

        private string BuildUserToken(string userName, int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            string s3 = Convert.ToBase64String(key);  // gsjqFw==

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public LoginResponse GetCurrentUser()
        {
            LoginResponse user = new LoginResponse
            {
                username = _accessor.HttpContext.User.Identity.Name
            };
            var userId = GetCurrentUserId();
            if(userId != null)
            {
                user.userId = userId.Value;
            }
            user.token = BuildUserToken(user.username, user.userId).ToString();

            return user;
        }

        private int? GetCurrentUserId()
        {
            var identity = (ClaimsIdentity)_accessor.HttpContext.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            foreach (var claim in claims)
            {
                var splitted = claim.Type.Split("/");
                if (splitted[splitted.Length - 1] == "nameidentifier")
                {
                    return Int32.Parse(claim.Value);
                }
            }
            return null;
        }
    }
}
