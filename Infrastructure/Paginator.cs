using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure
{
    public class Paginator<T> : BaseResponse<T>
    {
        public int PageNum { get; set; }
        public int AllPagesCount { get; set; }
        public int AllItemsCount { get; set; }
    }
}