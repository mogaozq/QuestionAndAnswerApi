using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using QandA.Data.Models;
using QuestionAndAnswerApi.Data.Models;
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

        public IEnumerable<QuestionModel> GetQuestions()
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select * from questions";
            return conn.Query<QuestionModel>(sqlCommand);
        }

        public IEnumerable<QuestionModel> GetQuestionsBySearch(string search)
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select * from Questions where Title ILIKE '%'||@Search||'%' or Content ILIKE '%'||@Search||'%'";
            return conn.Query<QuestionModel>(sqlCommand, new { Search = search });
        }
        public IEnumerable<QuestionModel> GetQuestionsBySearchWithPaging(string search, int page, int pageSize)
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select * from Questions where Title ILIKE '%'||@Search||'%' or Content ILIKE '%'||@Search||'%'" +
                " limit @Limit offset @Offset";
            return conn.Query<QuestionModel>(sqlCommand, new { Search = search, Limit = pageSize, Offset = (page - 1) * pageSize });
        }

        public IEnumerable<QuestionModel> GetUnansweredQuestions()
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select QuestionId, * from Questions as q where not exists (select * from answers where QuestionId = q.QuestionId)";
            return conn.Query<QuestionModel>(sqlCommand);
        }

        public IEnumerable<QuestionModel> GetUnansweredQuestionsWithPaged(int page, int pageSize)
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select QuestionId, * from Questions as q" +
                " where not exists (select * from answers where QuestionId = q.QuestionId)" +
                " limit @Limit offset @Offset";
            return conn.Query<QuestionModel>(sqlCommand, new { Limit = pageSize, Offset = (page - 1) * pageSize });
        }

        public async Task<IEnumerable<QuestionModel>> GetUnansweredQuestionsWithPagedAsync(int page, int pageSize)
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select QuestionId, * from Questions as q" +
                " where not exists (select * from answers where QuestionId = q.QuestionId)" +
                " limit @Limit offset @Offset";
            return await conn.QueryAsync<QuestionModel>(sqlCommand, new { Limit = pageSize, Offset = (page - 1) * pageSize });
        }
        //old method
        //public QuestionModel GetQuestion(int questionId)
        //{
        //    using var conn = OpenConnection(_connectionString);
        //    string sqlCommand = "select * from Questions where questionId=@QuestionId";
        //    var question = conn.QueryFirstOrDefault<QuestionModel>(sqlCommand, new { QuestionId = questionId });
        //    if (question is null)
        //        return null;

        //    string getQuestionAnswersSql = "select * from Answers where questionId=@QuestionId";
        //    question.Answers = conn.Query<AnswerModel>(getQuestionAnswersSql, new { QuestionId = questionId }).ToList();
        //    return question;
        //}

        public QuestionModel GetQuestion(int questionId)
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select * from Questions where questionId=@QuestionId;" +
                "select * from Answers where questionId=@QuestionId";
            using var result = conn.QueryMultiple(sqlCommand, new { QuestionId = questionId });
            var question = result.Read<QuestionModel>().SingleOrDefault();

            if (question != null)
                question.Answers = result.Read<AnswerModel>().ToList();

            return question;
        }

        public async Task<QuestionModel> GetQuestionAsync(int questionId)
        {
            using var conn = OpenConnection(_connectionString);
            string sqlCommand = "select * from Questions where questionId=@QuestionId;" +
                "select * from Answers where questionId=@QuestionId";
            using var result = await conn.QueryMultipleAsync(sqlCommand, new { QuestionId = questionId });
            var question = result.Read<QuestionModel>().SingleOrDefault();

            if (question != null)
                question.Answers = result.Read<AnswerModel>().ToList();

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

        public QuestionModel CreateQuestion(QuestionCreateModel questionModel)
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
        public QuestionModel UpdateQuestion(int questionId, QuestionUpdateModel questionModel)
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

        //old method
        //public IEnumerable<QuestionModel> GetQuestionsWithAnswers()
        //{
        //    using var conn = OpenConnection(_connectionString);
        //    var getQuestionsSql = "select * from questions";
        //    var questions = conn.Query<QuestionModel>(getQuestionsSql);

        //    foreach (var question in questions)
        //    {
        //        var getAnswers = "select * from answers where questionId = @QuestionId";
        //        question.Answers = conn.Query<AnswerModel>(getAnswers, new { QuestionId = question.QuestionId }).ToList();
        //    }
        //    return questions;
        //}

        public IEnumerable<QuestionModel> GetQuestionsWithAnswers()
        {
            var questionsDictionary = new Dictionary<int, QuestionModel>();
            using var conn = OpenConnection(_connectionString);


            var getQuestionsSql = "SELECT * FROM questions" +
                " left join answers" +
                " on questions.questionId = answers.questionId";

            conn.Query<QuestionModel, AnswerModel, QuestionModel>(getQuestionsSql, (q, a) =>
              {
                  if (questionsDictionary.TryGetValue(q.QuestionId, out var question))
                      question.Answers.Add(a);
                  else
                  {
                      q.Answers = new List<AnswerModel>();
                      q.Answers.Add(a);
                      questionsDictionary.Add(q.QuestionId, q);
                  }

                  return q;
              }, splitOn: nameof(AnswerModel.AnswerId));

            return questionsDictionary.Values;
        }

    }
}
