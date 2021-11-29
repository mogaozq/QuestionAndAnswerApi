using QandA.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data.EntityFrameworkCore.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
