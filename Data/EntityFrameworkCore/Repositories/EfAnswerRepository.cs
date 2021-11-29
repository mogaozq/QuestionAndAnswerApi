using Microsoft.EntityFrameworkCore;
using QandA.Data.Models;
using QuestionAndAnswerApi.Data.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data.EntityFrameworkCore.Repositories
{
    public class EfAnswerRepository : IAnswerRepository
    {
        private readonly EfDbContext _dbContext;

        public EfAnswerRepository(EfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AnswerModel CreateAnswer(AnswerCreateModel answerModel)
        {
            var answer = new Answer
            {
                QuestionId = answerModel.QuestionId,
                Content = answerModel.Content,
                CreatedAt = answerModel.CreatedAt,
                UserId = answerModel.UserId,
                UserName = answerModel.UserName
            };

            _dbContext.Answers.Add(answer);
            _dbContext.SaveChanges();

            return new AnswerModel
            {
                AnswerId = answer.AnswerId,
                Content = answer.Content,
                CreatedAt = answer.CreatedAt,
                UserName = answer.UserName,
            };
        }

        public AnswerModel GetAnswer(int answerId)
        {
            var answer = _dbContext.Answers.SingleOrDefault(a => a.AnswerId == answerId);
            if (answer == null) return null;

            return new AnswerModel
            {
                AnswerId = answer.AnswerId,
                Content = answer.Content,
                CreatedAt = answer.CreatedAt,
                UserName = answer.UserName,
            };
        }
    }
}
