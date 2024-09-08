using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Impl
{
    public class ClaimService : IClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? FindByKey(string key)
        {
            string? value = _httpContextAccessor.HttpContext?.User?
                .FindFirst(key)?.Value;
            return value;
        }

        public string GetUserId()
        {
            return FindByKey(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
