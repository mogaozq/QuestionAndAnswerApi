using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using QandA.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static QuestionAndAnswerApi.Data.Dapper.Utils;

namespace QuestionAndAnswerApi.Data.Dapper.Repositories
{
    public class DpQuestionRepository : IQuestionRepository
    {
        private readonly string _connectionString;

        public DpQuestionRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public IEnumerable<QuestionWithoutAnswers> GetQuestions()
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select * from questions";
            return conn.Query<QuestionWithoutAnswers>(sqlCommand);
        }

        public IEnumerable<QuestionWithoutAnswers> GetQuestionsBySearch(string search)
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select * from Questions where Title ILIKE '%'||@Search||'%' or Content ILIKE '%'||@Search||'%'";
            return conn.Query<QuestionWithoutAnswers>(sqlCommand, new { Search = search });
        }

        public IEnumerable<QuestionWithoutAnswers> GetUnansweredQuestions()
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select QuestionId, * from Questions as q where not exists (select * from answers where QuestionId = q.QuestionId)";
            return conn.Query<QuestionWithoutAnswers>(sqlCommand);
        }

        public QuestionWithAnswers GetQuestion(int questionId)
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select * from Questions where questionId=@QuestionId";
            var question = conn.QueryFirstOrDefault<QuestionWithAnswers>(sqlCommand, new { QuestionId = questionId });
            if (question is null)
                return null;

            string getQuestionAnswersSql = "select * from Answers where questionId=@QuestionId";
            question.Answers = conn.Query<AnswerModel>(getQuestionAnswersSql, new { QuestionId = questionId });
            return question;
        }
        public bool QuestionExists(int questionId)
        {
            using var conn = OpenConnection(_connectionString);
            var sqlCommand = "select case when exists (select * from questions where questionId=@QuestionId) " +
                "then true " +
                "else false " +
                "end as Result";
            return conn.QueryFirst<bool>(sqlCommand, new { QuestionId = questionId });
        }

        public QuestionWithAnswers CreateQuestion(QuestionCreateModel questionModel)
        {
            using var conn = OpenConnection(_connectionString);
            var sql = @"insert into 
                        questions(title, content, username, userid, createdat)
                        values(@Title, @Content, @Username, @UserId, @CreatedAt)
                        returning questionId";

            var questionId = conn.QueryFirst<int>(sql, new
            {
                Title = questionModel.Title,
                Content = questionModel.Content,
                Username = questionModel.UserName,
                UserId = questionModel.UserId,
                CreatedAt = questionModel.CreatedAt.ToUniversalTime()
            });

            return GetQuestion(questionId);
        }
        public QuestionWithAnswers UpdateQuestion(int questionId, QuestionUpdateModel questionModel)
        {
            using var conn = OpenConnection(_connectionString);
            var sql = "update questions" +
                " set title = @Title, content = @Content" +
                " where questionId=@QuestionId";

            conn.Execute(sql, new
            {
                Title = questionModel.Title,
                Content = questionModel.Content,
                QuestionId = questionId
            });

            return GetQuestion(questionId);
        }

        public void DeleteQuestion(int questionId)
        {
            using var conn = OpenConnection(_connectionString);
            var sql = "delete from questions where questionId=@QuestionId";

            conn.Execute(sql, new { QuestionId = questionId });
        }

    }
}
