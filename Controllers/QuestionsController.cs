using Microsoft.AspNetCore.Mvc;
using QandA.Data.Models;
using QuestionAndAnswerApi.Data;
using QuestionAndAnswerApi.Data.Cache;
using QuestionAndAnswerApi.Data.Models;
using QuestionAndAnswerApi.Models;
using QuestionAndAnswerApi.Requests;
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
        private readonly IQuestionCache _cache;

        public QuestionsController(IQuestionRepository questionRepo, IAnswerRepository answerRepo, IQuestionCache cache)
        {
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _cache = cache;
        }

        [HttpGet]
        public IEnumerable<QuestionModel> GetQuestion(string search, bool includeAnswers = false, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(search))
            {
                if (includeAnswers)
                    return _questionRepo.GetQuestionsWithAnswers();
                else
                    return _questionRepo.GetQuestions();
            }
            else
                return _questionRepo.GetQuestionsBySearchWithPaging(search, page, pageSize);
        }

        [HttpGet("{questionId}")]
        public ActionResult<QuestionModel> GetQuestions(int questionId)
        {
            var question = _cache.GetQuestion(questionId);
            if (question != null)
                return question;

            question = _questionRepo.GetQuestion(questionId);
            if (question == null)
                return NotFound();

            _cache.Set(question);
            return question;
        }

        

        [HttpPost]
        public ActionResult<QuestionModel> CreateQuestion(QuestionCreateUpdateDto questionCreateDto)
        {
            var question = _questionRepo.CreateQuestion(new QuestionCreateModel
            {
                Title = questionCreateDto.Title,
                Content = questionCreateDto.Content,
                UserId = "1",
                UserName = "moga@gmail.com",
                CreatedAt = DateTimeOffset.UtcNow
            });

            _cache.Set(question);
            return CreatedAtAction(nameof(GetQuestion), new { questionId = question.QuestionId }, question);
        }


        [HttpPut("{questionId}")]
        public ActionResult<QuestionModel> UpdateQuestion(int questionId, QuestionCreateUpdateDto questionCreateDto)
        {
            var question = _questionRepo.GetQuestion(questionId);
            if (question == null)
                return NotFound();

            var updatedQuestion = _questionRepo.UpdateQuestion(questionId, new QuestionUpdateModel
            {
                Title = questionCreateDto.Title,
                Content = questionCreateDto.Content
            });

            _cache.Remove(questionId);
            return updatedQuestion;
        }

        [HttpDelete("{questionId}")]
        public ActionResult<QuestionModel> DeleteQuestion(int questionId)
        {
            var question = _questionRepo.GetQuestion(questionId);
            if (question == null)
                return NoContent();

            _questionRepo.DeleteQuestion(questionId);

            _cache.Remove(questionId);
            return NoContent();
        }

        [HttpGet("unanswered")]
        public async Task<IEnumerable<QuestionModel>> GetUnansweredQuestions([FromQuery] RequestParams parameters)
        {
            return await _questionRepo.GetUnansweredQuestionsWithPagedAsync(parameters.Page, parameters.PageSize);
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
                CreatedAt = DateTimeOffset.UtcNow,
                UserId = "1",
                UserName = "moga@gmail.com"
            });

            return answer;
        }

    }
}
