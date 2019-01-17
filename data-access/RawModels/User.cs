using SQE.Backend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQE.Backend.DataAccess.RawModels
{
    internal class UserQueryResponse: IQueryResponse<UserData>
    {
        public string user_name { get; set; }
        public int user_id { get; set; }

        public UserData CreateModel()
        {
            return new UserData
            {
                Username = user_name,
                UserId = user_id
            };
        }
    }
}
