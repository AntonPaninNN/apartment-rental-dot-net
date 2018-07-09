using EntityContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;

namespace ApartmentRentalWeb.Controllers
{
    public class TestController : ApiController
    {

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public decimal Price { get; set; }
        }

        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };


        private readonly UnitOfWork _unit;

        public TestController()
        {
            _unit = new UnitOfWork();
        }

        [AllowAnonymous]
        public IEnumerable<Product> GetAllUsers()
        {
            // return CreateHttpResponse(request, () =>
            // {
            //HttpResponseMessage response = null;
            //IEnumerable<Model.User> users = _unit.Users.GetAll();
            //string res = string.Empty;
            //foreach (Model.User user in users)
            //{
            //    res += user.Login + " ";
            //}
            //response = request.CreateResponse<IEnumerable<Model.User>>(HttpStatusCode.OK, users);
            return products;
            // });
        }
    }
}
