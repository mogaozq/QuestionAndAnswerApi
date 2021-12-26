using Dapper;
using Microsoft.Extensions.Configuration;
using QandA.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static QuestionAndAnswerApi.Data.Dapper.Utils;

namespace QuestionAndAnswerApi.Data.Dapper.Repositories
{
    public class DpAnswerRepository : IAnswerRepository
    {
        private readonly string _connectionString;

        public DpAnswerRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public AnswerModel CreateAnswer(AnswerCreateModel answer)
        {
            using var conn = OpenConnection(_connectionString);
            var sql = "insert into" +
                " answers (content,userid,username,createdAt,questionId)" +
                " values (@Content,@UserId,@Username,@CreateAt,@QuestionId) returning * ";

            return  conn.QueryFirst<AnswerModel>(sql, new
            {
                Content = answer.Content,
                UserId = answer.UserId,
                Username = answer.UserName,
                CreateAt = answer.CreatedAt.ToUniversalTime(),
                QuestionId = answer.QuestionId,
            });
        }

        public AnswerModel GetAnswer(int answerId)
        {
            using var conn = OpenConnection(_connectionString);
            var sql = "select * from Answers where answerId=@AnswerId";
            return conn.QueryFirstOrDefault<AnswerModel>(sql, new { AnswerId = answerId });
        }
    }
}
