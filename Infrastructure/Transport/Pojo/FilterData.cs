using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartmentRentalWeb.Infrastructure.Transport.Pojo
{
    public class FilterData
    {
        public FilterText FirstName { get; set; }
        public FilterText LastName { get; set; }
        public FilterText Email { get; set; }
        public FilterDate DateOfBirth { get; set; }
        public FilterText Mobile { get; set; }
        public FilterText PassportData { get; set; }
        public FilterText Notes { get; set; }
    }
}