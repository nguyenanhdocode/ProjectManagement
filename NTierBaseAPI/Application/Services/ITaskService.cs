using Application.Common.Tree;
using Application.Models.Task;
using Core.Entities;
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

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<string>> Update(UpdateTaskModel model, bool updateEndDateOnly = false);

        /// <summary>
        /// Delete task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(string id);

        /// <summary>
        /// Create sub-task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CreateSubtaskReponseModel> CreateSubTask(CreateSubtaskModel model);

        /// <summary>
        /// Update task status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateStatus(UpdateTaskStatusModel model);
    }
}
