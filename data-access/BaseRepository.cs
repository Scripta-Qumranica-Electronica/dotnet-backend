using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SQE.Backend.DataAccess
{
    public class BaseRepository
    {
        protected IConfiguration _config;

        public BaseRepository(IConfiguration config)
        {
            _config = config;
        }

        protected string ConnectionString
        {
            get
            {
                return _config.GetConnectionString("DefaultConnection");
            }
        }

        protected IDbConnection OpenConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
