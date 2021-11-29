using QandA.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data
{
    public interface IQuestionRepository
    {
        QuestionWithAnswers GetQuestion(int questionId);

        IEnumerable<QuestionWithoutAnswers> GetQuestions();

        IEnumerable<QuestionWithoutAnswers> GetQuestionsBySearch(string search);

        IEnumerable<QuestionWithoutAnswers> GetUnansweredQuestions();

        bool QuestionExists(int questionId);

        QuestionWithAnswers CreateQuestion(QuestionCreateModel questionModel);

        QuestionWithAnswers UpdateQuestion(int questionId, QuestionUpdateModel questionModel);

        void DeleteQuestion(int questionId);

    }
}
