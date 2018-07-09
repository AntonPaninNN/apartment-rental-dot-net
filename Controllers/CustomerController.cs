using ApartmentRentalWeb.Infrastructure;
using ApartmentRentalWeb.Infrastructure.Transport.Pojo;
using ApartmentRentalWeb.Infrastructure.Transport.Requests;
using EntityContext;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebUtils.Constants;
using WebUtils.Encryption.Enums;

namespace ApartmentRentalWeb.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomerController : BaseController
    {
        public CustomerController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        private static Dictionary<string, List<Customer>> FilteredCustomers = new Dictionary<string, List<Customer>>();

        [HttpGet]
        [HttpPost]
        [Route("getAll")]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            try
            {
                IEnumerable<Customer> customers = _unitOfWork.Customers.GetAll();
                GeneralResponse<Customer> response = new GeneralResponse<Customer>()
                {
                    Items = customers,
                    Count = customers.Count()
                };
                return request.CreateResponse<GeneralResponse<Customer>>(HttpStatusCode.OK, response);
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [Route("get/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            try
            {
                Customer customer = _unitOfWork.Customers.Get(id);
                return request.CreateResponse<Customer>(HttpStatusCode.OK, customer);
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, FeedRequest<Customer> data)
        {
            try
            {
                Customer currentCustomer = _unitOfWork.Customers.Get(data.Item.ID);
                currentCustomer.DateOfBirth = data.Item.DateOfBirth;
                currentCustomer.Email = data.Item.Email;
                currentCustomer.FirstName = data.Item.FirstName;
                currentCustomer.LastName = data.Item.LastName;
                currentCustomer.Mobile = data.Item.Mobile;
                currentCustomer.Notes = data.Item.Notes;
                currentCustomer.PassportData = data.Item.PassportData;

                _unitOfWork.Customers.Update(currentCustomer);
                _unitOfWork.Save();
                string filterKey = GetCookieValue(request, AppConstants.LOGIN);

                if (FilteredCustomers.ContainsKey(filterKey))
                    FilteredCustomers.Remove(filterKey);

                return GetRange(request, new RangeRequest<Customer>()
                {
                    PageNum = data.PageNum,
                    PageSize = data.PageSize,
                    FieldName = data.FieldName,
                    Direction = data.Direction
                });
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpGet]
        [HttpPost]
        [Route("delete/{id:int}")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            try
            {
                _unitOfWork.Customers.Delete(id);
                _unitOfWork.Save();
                string filterKey = GetCookieValue(request, AppConstants.LOGIN);

                if (FilteredCustomers.ContainsKey(filterKey))
                    FilteredCustomers.Remove(filterKey);

                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpPost]
        [Route("delete")]
        public HttpResponseMessage DeleteList(HttpRequestMessage request, DeleteRequest<Customer> data)
        {
            try
            {
                foreach (int id in data.IDs)
                    _unitOfWork.Customers.Delete(id);

                _unitOfWork.Save();
                string filterKey = GetCookieValue(request, AppConstants.LOGIN);

                if (FilteredCustomers.ContainsKey(filterKey))
                    FilteredCustomers.Remove(filterKey);

                return GetRange(request, new RangeRequest<Customer>()
                {
                    PageNum = data.PageNum,
                    PageSize = data.PageSize,
                    FieldName = data.FieldName,
                    Direction = data.Direction
                });
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Create(HttpRequestMessage request, FeedRequest<Customer> data)
        {
            try
            {
                _unitOfWork.Customers.Create(data.Item);
                _unitOfWork.Save();
                string filterKey = GetCookieValue(request, AppConstants.LOGIN);

                if (FilteredCustomers.ContainsKey(filterKey))
                    FilteredCustomers.Remove(filterKey);

                return GetRange(request, new RangeRequest<Customer>()
                {
                    PageNum = data.PageNum,
                    PageSize = data.PageSize,
                    FieldName = data.FieldName,
                    Direction = data.Direction
                });
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpPost]
        [Route("range")]
        public HttpResponseMessage GetRange(HttpRequestMessage request, RangeRequest<Customer> data)
        {
            try
            {
                IEnumerable<Customer> customers = null;
                string filterKey = GetCookieValue(request, AppConstants.LOGIN);

                if (FilteredCustomers.ContainsKey(filterKey))
                    customers = FilteredCustomers[filterKey];
                else
                    customers = _unitOfWork.Customers.GetAll();

                int total = customers.Count();
                customers = SortCustomers(customers, data);
                customers = customers
                    .Skip((data.PageNum > 0 ? (data.PageNum - 1) : data.PageNum) * data.PageSize)
                    .Take(data.PageSize);
                
                Paginator<Customer> paginator = new Paginator<Customer>()
                {
                    Items = customers,
                    PageNum = data.PageNum,
                    AllItemsCount = total,
                    AllPagesCount = (int)Math.Ceiling((decimal)total / (decimal)data.PageSize)
                };

                return request.CreateResponse<Paginator<Customer>>(HttpStatusCode.OK, paginator);
            }
            catch (Exception ex) { return ProcessError(ex, request); }
            
        }

        [HttpPost]
        [Route("clearfilter")]
        public HttpResponseMessage ClearFilter(HttpRequestMessage request, RangeRequest<Customer> data)
        {
            try
            {
                string filterKey = GetCookieValue(request, AppConstants.LOGIN);

                if (FilteredCustomers.ContainsKey(filterKey))
                    FilteredCustomers.Remove(filterKey);

                return GetRange(request, new RangeRequest<Customer>()
                {
                    PageNum = data.PageNum,
                    PageSize = data.PageSize,
                    FieldName = data.FieldName,
                    Direction = data.Direction
                });
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpPost]
        [Route("filter")]
        public HttpResponseMessage Filter(HttpRequestMessage request, FilterRequest<Customer> data)
        {
            try
            {
                string filterKey = GetCookieValue(request, AppConstants.LOGIN);

                if (FilteredCustomers.ContainsKey(filterKey))
                    FilteredCustomers.Remove(filterKey);

                FilteredCustomers.Add(filterKey, _unitOfWork.Customers.GetAll()
                    .Where(c => CheckAllConditions(c, data)).ToList<Customer>());

                int total = FilteredCustomers[filterKey].Count();

                IEnumerable<Customer> customers = FilteredCustomers[filterKey];
                customers = SortCustomers(customers, data);
                customers = customers
                    .Skip((data.PageNum > 0 ? (data.PageNum - 1) : data.PageNum) * data.PageSize)
                    .Take(data.PageSize);
                
                Paginator<Customer> paginator = new Paginator<Customer>()
                {
                    Items = customers,
                    PageNum = data.PageNum,
                    AllItemsCount = total,
                    AllPagesCount = (int)Math.Ceiling((decimal)total / (decimal)data.PageSize)
                };

                return request.CreateResponse<Paginator<Customer>>(HttpStatusCode.OK, paginator);
            }
            catch (Exception ex) { return ProcessError(ex, request); }
        }

        [HttpPost]
        [Route("sort")]
        public HttpResponseMessage Sort(HttpRequestMessage request, BaseRequest<Customer> data)
        {
            return GetRange(request, new RangeRequest<Customer>()
            {
                PageNum = data.PageNum,
                PageSize = data.PageSize,   
                FieldName = data.FieldName,
                Direction = data.Direction
            });
        }

        protected IEnumerable<Customer> SortCustomers(IEnumerable<Customer> customers, BaseRequest<Customer> data)
        {
            if (string.IsNullOrEmpty(data.FieldName))
                return customers.OrderBy(c => c.ID);
            else
                return SortByFieldName(customers, data.FieldName, data.Direction);
        }

        protected IEnumerable<Customer> SortByFieldName(IEnumerable<Customer> customers, string fieldName, string direction)
        {
            var propertyInfo = typeof(Customer).GetProperty(UppercaseFirst(fieldName));
            if (direction.Equals("asc"))
                return customers.OrderBy(x => propertyInfo.GetValue(x, null));
            else if (direction.Equals("desc"))
                return customers.OrderByDescending(x => propertyInfo.GetValue(x, null));
            else
                return customers.OrderBy(x => x.ID);
        }

        protected string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private bool CheckAllConditions(Customer customer, FilterRequest<Customer> data)
        {
            if (CheckCondition(customer.FirstName, data.Data.FirstName) == ConditionState.NOT_PASSED ||
                CheckCondition(customer.LastName, data.Data.LastName) == ConditionState.NOT_PASSED ||
                CheckCondition(customer.Email, data.Data.Email) == ConditionState.NOT_PASSED ||
                CheckCondition(customer.PassportData, data.Data.PassportData) == ConditionState.NOT_PASSED ||
                CheckCondition(customer.Mobile, data.Data.Mobile) == ConditionState.NOT_PASSED ||
                CheckCondition(customer.Notes, data.Data.Notes) == ConditionState.NOT_PASSED ||
                CheckCondition(customer.DateOfBirth, data.Data.DateOfBirth) == ConditionState.NOT_PASSED)
                return false;
            else
                return true;
        }

        private ConditionState CheckCondition(object field, IFilterEntity entity)
        {
            if (entity == null)
                return ConditionState.NOT_USED;

            if (entity is FilterText)
            {
                FilterText innerEntity = (FilterText)entity;
                if (string.IsNullOrEmpty(innerEntity.Condition) || string.IsNullOrEmpty(innerEntity.Value))
                    return ConditionState.NOT_USED;

                string innerField = Convert.ToString(field);
                if (innerEntity.Condition.Equals("contains"))
                    return innerField.Contains(innerEntity.Value) ? ConditionState.PASSED : ConditionState.NOT_PASSED;
                else if (innerEntity.Condition.Equals("equals"))
                    return innerField.Equals(innerEntity.Value) ? ConditionState.PASSED : ConditionState.NOT_PASSED;
                else
                    return ConditionState.NOT_USED;
            }
            else if (entity is FilterDate)
            {
                FilterDate innerEntity = (FilterDate)entity;
                if (innerEntity.From == null || innerEntity.To == null)
                    return ConditionState.NOT_USED;

                DateTime innerField = Convert.ToDateTime(field);
                DateTime from = DateTime.Parse(innerEntity.From);
                DateTime to = DateTime.Parse(innerEntity.To);

                int fromResult = DateTime.Compare(from, innerField);
                if (fromResult <= 0)
                {
                    int toResult = DateTime.Compare(innerField, to);
                    if (toResult <= 0)
                        return ConditionState.PASSED;
                }
                return ConditionState.NOT_PASSED;
            }
            else
                return ConditionState.NOT_PASSED;
        }
    }

    public enum ConditionState
    {
        PASSED,
        NOT_PASSED,
        NOT_USED
    }
}
