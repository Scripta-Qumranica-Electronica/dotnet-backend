using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQE.Backend.DTOs
{
    public class UserInformation
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
