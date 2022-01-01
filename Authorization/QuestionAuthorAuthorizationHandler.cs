using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using QuestionAndAnswerApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Authorization
{
    public class QuestionAuthorAuthorizationHandler : AuthorizationHandler<QuestionAuthorRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuestionRepository _questionRepo;

        public QuestionAuthorAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IQuestionRepository questionRepo)
        {
            _httpContextAccessor = httpContextAccessor;
            _questionRepo = questionRepo;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, QuestionAuthorRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var questionIdObject = _httpContextAccessor.HttpContext.Request.RouteValues["questionId"];
            var questionId = Convert.ToInt32(questionIdObject);

            var question = await _questionRepo.GetQuestionAsync(questionId);
            if (question == null)
            {
                context.Succeed(requirement);
                return;
            }

            if (question.UserId != userId)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}
