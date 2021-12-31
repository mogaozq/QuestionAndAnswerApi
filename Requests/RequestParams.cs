using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Requests
{
    public class RequestParams
    {
        private const int MAX_PAGESIZE = 50;
        private int _pageSize = 10;
        public int Page { get; set; } = 1;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value <= MAX_PAGESIZE & value > 0)
                    _pageSize = value;
            }
        } 
    }
}
