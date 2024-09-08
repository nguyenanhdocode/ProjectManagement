using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Impl
{
    public class TemplateService : ITemplateService
    {
        private readonly string _templatePath;

        public TemplateService()
        {
            var projectPath = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
            if (projectPath == null)
                throw new Exception("Project path is not found");

            var templateProject = Assembly.GetExecutingAssembly().GetName().Name;

            if (templateProject == null)
                throw new Exception("Template project is not found");

            _templatePath = Path.Combine(projectPath, templateProject, "Templates");
        }

        public async Task<string> GetTemplateAsync(string templateName)
        {
            using var reader = new StreamReader(Path.Combine(_templatePath, templateName));

            return await reader.ReadToEndAsync();
        }

        public string ReplaceInTemplate(string input, IDictionary<string, string> replaceWords)
        {
            var response = input;

            foreach (var temp in replaceWords)
                response = response.Replace(temp.Key, temp.Value);

            return response;
        }
    }
}
