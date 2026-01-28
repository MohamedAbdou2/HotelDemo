using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Helper
{
    public class CurrentUser
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
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
    }
}
