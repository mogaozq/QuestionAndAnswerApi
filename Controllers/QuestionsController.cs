using Microsoft.AspNetCore.Mvc;
using QandA.Data.Models;
using QuestionAndAnswerApi.Data;
using QuestionAndAnswerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IAnswerRepository _answerRepo;

        public QuestionsController(IQuestionRepository questionRepo, IAnswerRepository answerRepo)
        {
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
        }

        [HttpGet]
        public IEnumerable<QuestionWithoutAnswers> GetQuestion(string search)
        {
            if (string.IsNullOrEmpty(search))
                return _questionRepo.GetQuestions();
            else
                return _questionRepo.GetQuestionsBySearch(search);
        }

        [HttpGet("{questionId}")]
        public ActionResult<QuestionWithAnswers> GetQuestions(int questionId)
        {
            var question = _questionRepo.GetQuestion(questionId);
            if (question == null)
                return NotFound();

            return question;
        }

        [HttpGet("unanswered")]
        public IEnumerable<QuestionWithoutAnswers> GetUnansweredQuestions()
        {
            return _questionRepo.GetUnansweredQuestions();
        }

        [HttpPost]
        public ActionResult<QuestionWithAnswers> CreateQuestion(QuestionCreateUpdateDto questionCreateDto)
        {
            var question = _questionRepo.CreateQuestion(new QuestionCreateModel
            {
                Title = questionCreateDto.Title,
                Content = questionCreateDto.Content,
                UserId = "1",
                UserName = "moga@gmail.com",
                CreatedAt = DateTimeOffset.UtcNow
            });

            return CreatedAtAction(nameof(GetQuestion), new { questionId = question.QuestionId }, question);
        }


        [HttpPut("{questionId}")]
        public ActionResult<QuestionWithAnswers> UpdateQuestion(int questionId, QuestionCreateUpdateDto questionCreateDto)
        {
            var question = _questionRepo.GetQuestion(questionId);
            if (question == null)
                return NotFound();

            var updatedQuestion = _questionRepo.UpdateQuestion(questionId, new QuestionUpdateModel
            {
                Title = questionCreateDto.Title,
                Content = questionCreateDto.Content
            });

            return updatedQuestion;
        }

        [HttpDelete("{questionId}")]
        public ActionResult<QuestionWithAnswers> DeleteQuestion(int questionId)
        {
            var question = _questionRepo.GetQuestion(questionId);
            if (question == null)
                return NoContent();

            _questionRepo.DeleteQuestion(questionId);

            return NoContent();
        }

        [HttpPost("{questionId}/answers")]
        public ActionResult<AnswerModel> CreateAnswer(int questionId, CreateUpdateAnswerDto createAnswerDto)
        {
            if (!_questionRepo.QuestionExists(questionId))
                return NotFound();

            var answer = _answerRepo.CreateAnswer(new AnswerCreateModel
            {
                QuestionId = questionId,
                Content = createAnswerDto.Content,
                CreatedAt= DateTimeOffset.UtcNow,
                UserId="1",
                UserName="moga@gmail.com"
            });

            return answer;
        }

    }
}
