using EntityContext;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebUtils.Constants;
using WebUtils.Encryption;
using WebUtils.Encryption.Factory;

namespace ApartmentRentalWeb.Controllers
{
    [RoutePrefix("api/account")]
    public class UserController : BaseController
    {
        public UserController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Route("getAll")]
        public HttpResponseMessage GetAllUsers(HttpRequestMessage request)
        {
            try
            {
                IEnumerable<User> users = _unitOfWork.Users.GetAll();
                return request.CreateResponse<IEnumerable<User>>(HttpStatusCode.OK, users);
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpPost]
        [Route("login")]
        public HttpResponseMessage Login(HttpRequestMessage request, User user)
        {
            try
            {
                User targetUser = _unitOfWork.Users.GetAll().FirstOrDefault(x => x.Login.Equals(user.Login));
                if (targetUser != null)
                {
                    if (targetUser.HashedPassword.Equals(user.HashedPassword))
                    {
                        string encryptedLogin = Encrypt(user.Login);
                        CookieHeaderValue cookie = CreateCookie(AppConstants.LOGIN, encryptedLogin, DateTimeOffset.Now.AddDays(1));
                        HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
                        response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                        return response;
                    }
                }
                return request.CreateResponse<String>(HttpStatusCode.Unauthorized, "Wrong login or password");
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpPost]
        [Route("signin")]
        public HttpResponseMessage SignIn(HttpRequestMessage request, User user)
        {
            try
            {
                /* FAKE */
                user.LockedFrom = DateTime.Now;
                user.LockedTo = DateTime.Now;
                /* FAKE */

                User targetUser = _unitOfWork.Users.GetAll().FirstOrDefault(x => x.Login.Equals(user.Login));
                if (targetUser != null)
                {
                    return request.CreateResponse<String>(HttpStatusCode.Unauthorized, "User with that login already exists");
                }

                _unitOfWork.Users.Create(user);
                _unitOfWork.Save();
                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ProcessError(ex, request);
            }
        }
    }
}
