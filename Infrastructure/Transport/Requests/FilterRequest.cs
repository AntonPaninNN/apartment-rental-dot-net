using ApartmentRentalWeb.Infrastructure.Transport.Pojo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure.Transport.Requests
{
    public class FilterRequest<T> : BaseRequest<T>
    {
        public FilterData Data { get; set; }
    }
}