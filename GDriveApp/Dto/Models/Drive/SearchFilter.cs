using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Models.Drive
{
    public class SearchFilter
    {
        public SearchFilter()
        {
            PageSize = 20;
        }

        public int PageSize { get; set; }

        public string Query { get; set; }
    }
}
