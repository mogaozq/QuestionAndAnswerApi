using QuestionAndAnswerApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data.Cache
{
    public interface IQuestionCache
    {
        QuestionModel GetQuestion(int questionId);

        void Set(QuestionModel question);
        void Remove(int questionId);

    }
}
