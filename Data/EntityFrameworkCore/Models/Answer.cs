using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data.EntityFrameworkCore.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int QuestionId { get; set; }

        public Question Question { get; set; }
    }
}
