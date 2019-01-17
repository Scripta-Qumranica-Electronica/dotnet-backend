using SQE.Backend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SQE.Backend.DataAccess.RawModels;
using Dapper;
using System.Linq;

namespace SQE.Backend.DataAccess
{
    public interface IUserRepository
    {
        Task<UserData> login(string userName, string password);
    }

    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration config) : base(config) { }

        public async Task<UserData> login(string user_name, string password)
        {
            var sql = @"SELECT user_id, user_name FROM user WHERE user_name =@UserName AND pw = SHA2(@Password, 224)";
            UserData user = new UserData();
            using (var connection = OpenConnection())
            {
                var results = await connection.QueryAsync<UserQuery>(sql, new
                {
                    UserName = user_name,
                    Password = password
                });
                var firstUser = results.FirstOrDefault();
                if (firstUser != null)
                {
                    user.UserId = firstUser.user_id;
                    user.Username = firstUser.user_name;
                }
                return user;
            }
        }
    }
}
