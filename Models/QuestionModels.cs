using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Models
{
    public class QuestionCreateUpdateDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Content { get; set; }
    }

    public class CreateUpdateAnswerDto
    {
        [Required]
        [StringLength(100)]
        public string Content { get; set; }
    }
}
