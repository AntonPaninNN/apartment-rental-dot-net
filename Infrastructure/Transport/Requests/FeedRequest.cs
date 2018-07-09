using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure
{
    public class FeedRequest<T> : BaseRequest<T>
    {
        public T Item { get; set; }
    }
}