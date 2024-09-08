using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ITemplateService
    {
        /// <summary>
        /// Get html template
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        Task<string> GetTemplateAsync(string templateName);

        /// <summary>
        /// Replace content in template
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replaceWords"></param>
        /// <returns></returns>
        string ReplaceInTemplate(string input, IDictionary<string, string> replaceWords);
    }
}
