using Microsoft.Extensions.Caching.Memory;
using QuestionAndAnswerApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data.Cache
{
    public class QuestionCache : IQuestionCache
    {
        private readonly MemoryCache _cache;

        public QuestionCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 100 });
        }

        private string GetCacheKey(int questionId) => $"Question-{questionId}";

        public QuestionModel GetQuestion(int questionId)
        {
            _cache.TryGetValue(GetCacheKey(questionId), out QuestionModel question);
            return question;
        }

        public void Remove(int questionId)
        {
            _cache.Remove(GetCacheKey(questionId));
        }

        public void Set(QuestionModel question)
        {
            _cache.Set(GetCacheKey(question.QuestionId), question, new MemoryCacheEntryOptions
            {
                Size = 1,
            });
        }
    }
}
