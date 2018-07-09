using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure.Transport.Pojo
{
    public class FilterDate : IFilterEntity
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}