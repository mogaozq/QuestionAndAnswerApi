using Microsoft.EntityFrameworkCore;
using QandA.Data.Models;
using QuestionAndAnswerApi.Data.EntityFrameworkCore.Models;
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

        public IEnumerable<QuestionWithoutAnswers> GetQuestions()
        {
            var questions = _dbContext.Questions.AsNoTracking().ToArray();
            return questions.Select(q => new QuestionWithoutAnswers
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserName = q.UserName
            });
        }

        public IEnumerable<QuestionWithoutAnswers> GetQuestionsBySearch(string search)
        {
            //todo: how to do full text search instead of following approach
            search = search.ToLower().Trim();
            var questions = _dbContext.Questions.AsNoTracking().Where(q => q.Title.ToLower().Contains(search) || q.Content.ToLower().Contains(search)).ToArray();
            return questions.Select(q => new QuestionWithoutAnswers
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserName = q.UserName
            });
        }

        public QuestionWithAnswers GetQuestion(int questionId)
        {
            var question = _dbContext
                .Questions.AsNoTracking()
                .Include(q => q.Answers)
                .SingleOrDefault(q => q.QuestionId == questionId);

            if (question == null)
                return null;

            return new QuestionWithAnswers
            {
                QuestionId = question.QuestionId,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                UserName = question.UserName,
                Answers = question.Answers.Select(a => new AnswerModel
                {
                    AnswerId = a.AnswerId,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    UserName = a.UserName
                })
            };
        }

        public IEnumerable<QuestionWithoutAnswers> GetUnansweredQuestions()
        {
            var questions = _dbContext.Questions.AsNoTracking()
                .Where(q => q.Answers.Count() == 0)
                .ToArray();

            return questions.Select(q => new QuestionWithoutAnswers
            {
                QuestionId = q.QuestionId,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserName = q.UserName
            });
        }

        public QuestionWithAnswers CreateQuestion(QuestionCreateModel questionModel)
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

            return new QuestionWithAnswers
            {
                QuestionId = question.QuestionId,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                UserName = question.UserName,
                Answers = Array.Empty<AnswerModel>()
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

        public QuestionWithAnswers UpdateQuestion(int questionId, QuestionUpdateModel questionModel)
        {
            var question = _dbContext.Questions.Single(q => q.QuestionId == questionId);
            question.Title = questionModel.Title;
            question.Content = questionModel.Content;
            _dbContext.SaveChanges();

            return GetQuestion(questionId);
        }
    }
}
