using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace Application.Helper
{
    public class CurrentUser
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IGenericRepository<Domain.Models.Customer> _customerRepo;
        public CurrentUser(IHttpContextAccessor httpContextAccessor, IGenericRepository<Domain.Models.Customer> customerRepo)
        {
            this.httpContextAccessor = httpContextAccessor;
            this._customerRepo = customerRepo;
        }
        public Guid? GetUserId()
        {
            var currentUserIdString = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? currentUserId = null;
            if (Guid.TryParse(currentUserIdString, out var parsedGuid))
            {
                currentUserId = parsedGuid;
            }

            return currentUserId;
        }

        public string GetUserRole()
        {
            var currentUserIdString = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            return currentUserIdString;
        }
        public Guid GetCustomerId(Guid userId)
        {
            var customerId = _customerRepo.GetAll(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();
            return customerId;
        }

    }
}
