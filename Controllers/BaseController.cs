using ApartmentRentalWeb.Infrastructure;
using EntityContext;
using EntityContext.Repositories;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebUtils.Encryption;
using WebUtils.Encryption.Factory;

namespace ApartmentRentalWeb.Controllers
{
    public class BaseController : ApiController
    {
        protected readonly IUnitOfWork _unitOfWork = null;

        public BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected HttpResponseMessage ProcessError(Exception ex, HttpRequestMessage request)
        {
            LogError(ex);
            return request.CreateResponse<String>(HttpStatusCode.InternalServerError, ex.Message);
        }

        protected void LogError(Exception ex)
        {
            try
            {
                Error error = new Error() { DateCreated = DateTime.Now, Message = ex.Message, StackTrace = ex.StackTrace };
                _unitOfWork.Errors.Create(error);
                _unitOfWork.Save();
            }
            catch { }
        }

        protected CookieHeaderValue CreateCookie(string name, string value, DateTimeOffset offset)
        {
            var cookie = new CookieHeaderValue(name, value);
            cookie.Expires = offset;
            cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";
            return cookie;
        }

        protected string Encrypt(string input)
        {
            IEncrypter encrypter = EncrypterFactory.CreateEncrypter(WebUtils.Encryption.Enums.EncrypterType.MD5);
            return encrypter.GetHash(input);
        }

        protected string GetCookieValue(HttpRequestMessage request, string name)
        {
            CookieHeaderValue cookie = request.Headers.GetCookies(name).FirstOrDefault();
            if (cookie != null)
                return cookie[name].Value;

            return null;
        }
    }
}
