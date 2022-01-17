using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QandA.Data.Models;
using QuestionAndAnswerApi.Data;
using QuestionAndAnswerApi.Data.Cache;
using QuestionAndAnswerApi.Data.Models;
using QuestionAndAnswerApi.Models;
using QuestionAndAnswerApi.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IAnswerRepository _answerRepo;
        private readonly IQuestionCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _userProfileUrl;

        public QuestionsController(IQuestionRepository questionRepo, IAnswerRepository answerRepo, IQuestionCache cache, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _cache = cache;
            _httpClientFactory = httpClientFactory;
            _userProfileUrl = $"{configuration["Auth0:Authority"]}userinfo";
        }

        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<QuestionModel> GetQuestions(string search, bool includeAnswers = false, int page = 1, int pageSize = 10)
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
        [AllowAnonymous]
        public ActionResult<QuestionModel> GetQuestion(int questionId)
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
        public async Task<ActionResult<QuestionModel>> CreateQuestionAsync(QuestionCreateUpdateDto questionCreateDto)
        {
            var question = _questionRepo.CreateQuestion(new QuestionCreateModel
            {
                Title = questionCreateDto.Title,
                Content = questionCreateDto.Content,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = (await GetUserInfoAsync()).Name,
                CreatedAt = DateTimeOffset.UtcNow
            });
                
            _cache.Set(question);
            return CreatedAtAction(nameof(GetQuestion), new { questionId = question.QuestionId }, question);
        }

        [HttpPut("{questionId}")]
        [Authorize("QuestionAuthor")]
        public ActionResult<QuestionModel> UpdateQuestion(int questionId, QuestionCreateUpdateDto questionCreateDto)
        {
            var question = _questionRepo.GetQuestionAsync(questionId);
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
        [Authorize("QuestionAuthor")]
        public ActionResult<QuestionModel> DeleteQuestion(int questionId)
        {
            var question = _questionRepo.GetQuestionAsync(questionId);
            if (question == null)
                return NoContent();

            _questionRepo.DeleteQuestion(questionId);

            _cache.Remove(questionId);
            return NoContent();
        }

        [HttpGet("unanswered")]
        [AllowAnonymous]
        public async Task<IEnumerable<QuestionModel>> GetUnansweredQuestions([FromQuery] RequestParams parameters)
        {
            return await _questionRepo.GetUnansweredQuestionsWithPagedAsync(parameters.Page, parameters.PageSize);
        }

        [HttpPost("{questionId}/answers")]
        public async Task<ActionResult<AnswerModel>> CreateAnswerAsync(int questionId, CreateUpdateAnswerDto createAnswerDto)
        {
            if (!_questionRepo.QuestionExists(questionId))
                return NotFound();

            var answer = _answerRepo.CreateAnswer(new AnswerCreateModel
            {
                QuestionId = questionId,
                Content = createAnswerDto.Content,
                CreatedAt = DateTimeOffset.UtcNow,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = (await GetUserInfoAsync()).Name
            });

            _cache.Remove(questionId);

            return answer;
        }

        private async Task<User> GetUserInfoAsync()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, _userProfileUrl);
            httpRequest.Headers.Add("Authorization", Request.Headers["Authorization"].First());

            var client = _httpClientFactory.CreateClient();
            var httpResponse = await client.SendAsync(httpRequest);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContentString = await httpResponse.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<User>(responseContentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            throw new InvalidOperationException();
        }
    }
}
