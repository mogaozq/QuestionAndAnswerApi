using Microsoft.EntityFrameworkCore;
using QandA.Data.Models;
using QuestionAndAnswerApi.Data.EntityFrameworkCore.Models;
using QuestionAndAnswerApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data.EntityFrameworkCore.Repositories
{
    public class EfQuestionRepository : IQuestionRepository
    {
        private readonly EfDbContext _dbContext;

        public EfQuestionRepository(EfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<QuestionModel> GetQuestions()
        {
            return _dbContext.Questions.AsNoTracking()
                .Select(q => new QuestionModel
                {
                    QuestionId = q.QuestionId,
                    Title = q.Title,
                    Content = q.Content,
                    CreatedAt = q.CreatedAt,
                    UserName = q.UserName,
                    UserId = q.UserId
                }).ToArray();

        }

        public IEnumerable<QuestionModel> GetQuestionsBySearch(string search)
        {
            //todo: how to do full text search instead of following approach
            search = search.ToLower().Trim();
            var questions = _dbContext.Questions.AsNoTracking().Where(q => q.Title.ToLower().Contains(search) || q.Content.ToLower().Contains(search)).ToArray();
            return questions.Select(q => new QuestionModel
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserName = q.UserName,
                UserId = q.UserId
            });
        }

        public QuestionModel GetQuestion(int questionId)
        {
            var question = _dbContext
                .Questions.AsNoTracking()
                .SingleOrDefault(q => q.QuestionId == questionId);

            question.Answers = _dbContext.Answers.AsNoTracking().Where(a => a.QuestionId == questionId).ToList();

            if (question == null)
                return null;

            return new QuestionModel
            {
                QuestionId = question.QuestionId,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                UserName = question.UserName,
                UserId = question.UserId,
                Answers = question.Answers.Select(a => new AnswerModel
                {
                    AnswerId = a.AnswerId,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    UserName = a.UserName
                }).ToList()
            };
        }

        public IEnumerable<QuestionModel> GetUnansweredQuestions()
        {
            var questions = _dbContext.Questions.AsNoTracking()
                .Where(q => q.Answers.Count() == 0)
                .ToArray();

            return questions.Select(q => new QuestionModel
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserName = q.UserName,
                UserId = q.UserId
            });
        }

        public IEnumerable<QuestionModel> GetUnansweredQuestionsWithPaged(int page, int pageSize)
        {
            var questions = _dbContext.Questions.AsNoTracking()
                .Where(q => q.Answers.Count() == 0)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            return questions.Select(q => new QuestionModel
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserName = q.UserName,
                UserId = q.UserId
            });
        }
        public async Task<IEnumerable<QuestionModel>> GetUnansweredQuestionsWithPagedAsync(int page, int pageSize)
        {
            var questions = await _dbContext.Questions.AsNoTracking()
                .Where(q => q.Answers.Count() == 0)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return questions.Select(q => new QuestionModel
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserName = q.UserName,
                UserId = q.UserId
            });
        }

        public QuestionModel CreateQuestion(QuestionCreateModel questionModel)
        {
            var question = new Question
            {
                Title = questionModel.Title,
                Content = questionModel.Content,
                UserId = questionModel.Title,
                UserName = questionModel.UserName,
                CreatedAt = questionModel.CreatedAt,
            };

            _dbContext.Questions.Add(question);
            _dbContext.SaveChanges();

            return new QuestionModel
            {
                QuestionId = question.QuestionId,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                UserName = question.UserName,
                UserId = question.UserId,
                Answers = new List<AnswerModel>()
            };
        }

        public void DeleteQuestion(int questionId)
        {
            _dbContext.Questions.Remove(new Question { QuestionId = questionId });
            _dbContext.SaveChanges();
        }

        public bool QuestionExists(int questionId)
        {
            return _dbContext.Questions.Any(q => q.QuestionId == questionId);
        }

        public QuestionModel UpdateQuestion(int questionId, QuestionUpdateModel questionModel)
        {
            var question = _dbContext.Questions.Single(q => q.QuestionId == questionId);
            question.Title = questionModel.Title;
            question.Content = questionModel.Content;
            _dbContext.SaveChanges();

            return GetQuestion(questionId);
        }

        public IEnumerable<QuestionModel> GetQuestionsWithAnswers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QuestionModel> GetQuestionsBySearchWithPaging(string search, int page, int pageSize)
        {
            return _dbContext.Questions.AsNoTracking()
                .Where(q => q.Title.Contains(search) || q.Content.Contains(search))
                .Skip((page - 1) * pageSize)
                .Take(pageSize).Select(q => new QuestionModel
                {
                    QuestionId = q.QuestionId,
                    Title = q.Title,
                    Content = q.Content,
                    CreatedAt = q.CreatedAt,
                    UserName = q.UserName,
                    UserId = q.UserId,
                    Answers = new List<AnswerModel>()
                }).ToArray();
        }


    }
}
