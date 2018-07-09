using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure.Transport.Pojo
{
    public class FilterText : IFilterEntity
    {
        public string Condition { get; set; }
        public string Value { get; set; }
    }
}