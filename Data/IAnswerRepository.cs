using QandA.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data
{
    public interface IAnswerRepository
    {
        AnswerModel GetAnswer(int answerId);

        AnswerModel CreateAnswer(AnswerCreateModel answer);

    }
}
