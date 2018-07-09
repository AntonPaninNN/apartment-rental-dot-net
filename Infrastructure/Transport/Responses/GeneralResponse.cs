using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure
{
    public class GeneralResponse<T> : BaseResponse<T>
    {
        public int Count { get; set; }
    }
}