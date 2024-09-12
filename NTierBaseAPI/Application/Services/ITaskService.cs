using Application.Common.Tree;
using Application.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ITaskService
    {
        /// <summary>
        /// Create task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CreateTaskResponseModel> Create(CreateTaskModel model);

        /// <summary>
        /// Get preview information before update datetime of task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<PreviewBeforeUpdateEndDateResponseModel> GetPreviewBeforeUpdateEndDate(
            PreviewBeforeUpdateEndDateModel model);

    }
}
