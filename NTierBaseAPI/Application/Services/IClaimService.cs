using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IClaimService
    {
        /// <summary>
        /// Find claim by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string? FindByKey(string key);

        /// <summary>
        /// Get userId
        /// </summary>
        /// <returns></returns>
        string GetUserId();
    }
}
