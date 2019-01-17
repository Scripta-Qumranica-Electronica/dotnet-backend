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
        Task<UserInformation> AuthenticateAsync(string username, string password);
        UserInformation getCurrentUser();
        string getUserToken(string userName, string userId);
        string getCurrentUserId();
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


        public async Task<UserInformation> AuthenticateAsync(string username, string password)
        {
            var result = await _repo.login(username, password);
            if(result.UserId ==null || result.Username == null)
            {
                return null;
            }
            var user = new UserInformation { Username = result.Username, userId = result.UserId };

            user.Token = getUserToken(user.Username, user.userId).ToString();

            // remove password before returning
            user.Password = null;

            return user;
        }

        public string getUserToken(string userName, string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            string s3 = Convert.ToBase64String(key);  // gsjqFw==

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public UserInformation getCurrentUser()
        {
            var user = new UserInformation
            {
                Username = _accessor.HttpContext.User.Identity.Name

            };
            var userId = getCurrentUserId();
            if(userId != null)
            {
                user.userId = userId.ToString();
            }
            user.Token = getUserToken(user.Username, user.userId).ToString();
            return user;
        }

        public string getCurrentUserId()
        {
            var identity = (ClaimsIdentity)_accessor.HttpContext.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            foreach (var claim in claims)
            {
                var splitted = claim.Type.Split("/");
                if (splitted[splitted.Length - 1] == "nameidentifier")
                {
                    return claim.Value.ToString();
                }
            }
            return null;
        }
    }
}
