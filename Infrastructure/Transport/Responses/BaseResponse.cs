using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure
{
    public class BaseResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
    }
}