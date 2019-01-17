using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SQE.Backend.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }

    public class LoginResponse
    {
        public string username { get; set; }
        public string token { get; set; }
        public int userId { get; set; }
    }
}
