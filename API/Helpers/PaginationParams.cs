using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationParams
    {
         public int PageNumber { get; set; }=1;
        private int _pageSize=10;
        private const int _maxPageSize=50;
        public int PageSize { get => _pageSize; set=>_pageSize=value>_maxPageSize?_pageSize:value; }
    }
}