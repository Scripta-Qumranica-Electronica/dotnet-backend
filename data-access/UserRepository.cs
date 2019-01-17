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
        Task<UserData> GetUserByPassword(string userName, string password);
    }

    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration config) : base(config) { }

        public async Task<UserData> GetUserByPassword(string userName, string password)
        {
            var sql = @"SELECT user_id, user_name FROM user WHERE user_name =@UserName AND pw = SHA2(@Password, 224)";
            using (var connection = OpenConnection())
            {
                var results = await connection.QueryAsync<UserQueryResponse>(sql, new
                {
                    UserName = userName,
                    Password = password
                });

                var firstUser = results.FirstOrDefault();
                if (firstUser == null)
                    return null;

                return firstUser.CreateModel();
            }
        }
    }
}
