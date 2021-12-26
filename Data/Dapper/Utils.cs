using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data.Dapper
{
    public static class Utils
    {
        public static IDbConnection OpenConnection(string connectionString)
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
