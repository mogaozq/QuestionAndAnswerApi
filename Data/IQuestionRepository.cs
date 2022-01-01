using QandA.Data.Models;
using QuestionAndAnswerApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data
{
    public interface IQuestionRepository
    {
        QuestionModel GetQuestion(int questionId);
        Task<QuestionModel> GetQuestionAsync(int questionId);

        IEnumerable<QuestionModel> GetQuestions();

        IEnumerable<QuestionModel> GetQuestionsWithAnswers();
        IEnumerable<QuestionModel> GetQuestionsBySearch(string search);
        IEnumerable<QuestionModel> GetQuestionsBySearchWithPaging(string search, int page, int pageSize);
        IEnumerable<QuestionModel> GetUnansweredQuestions();
        IEnumerable<QuestionModel> GetUnansweredQuestionsWithPaged(int page, int pageSize);
        Task<IEnumerable<QuestionModel>> GetUnansweredQuestionsWithPagedAsync(int page, int pageSize);

        bool QuestionExists(int questionId);

        QuestionModel CreateQuestion(QuestionCreateModel questionModel);

        QuestionModel UpdateQuestion(int questionId, QuestionUpdateModel questionModel);

        void DeleteQuestion(int questionId);

    }
}
