using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils.Encryption.Enums;

namespace ApartmentRentalWeb.Infrastructure
{
    public class BaseRequest<T>
    {
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public string FieldName { get; set; }
        public string Direction { get; set; }
    }
}