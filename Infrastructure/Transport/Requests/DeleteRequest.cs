using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure
{
    public class DeleteRequest<T> : BaseRequest<T>
    {
        public int[] IDs { get; set; }
    }
}