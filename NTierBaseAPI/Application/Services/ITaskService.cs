using Application.Common;
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
        Task<List<string>> Update(UpdateTaskModel model, bool updateEndDateOnly = false, bool skipFirst = false);

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

        /// <summary>
        /// Get task overview
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ViewTaskOverviewResponseModel> GetOvervew(ViewTaskOverviewModel model);

        /// <summary>
        /// Get task model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ViewTaskModel> GetTask(string id);

        /// <summary>
        /// Check before update enddate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CheckAffectBeforeUpdateEndDate(CheckAffectBeforeUpdateEndDateModel model);

        /// <summary>
        /// Get allowed new status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<AppTaskStatus>> GetAllowedNewStatus(ViewAllowedNewTaskStatusModel model);

        /// <summary>
        /// Get subtasks of task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<ViewTaskModel>> GetSubtasks(ViewSubtasksModel model);

        /// <summary>
        /// Update subtask model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateSubtask(UpdateSubtaskModel model);

        /// <summary>
        /// Check can update enddate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CheckCanUpdateEndDate(CheckCanUpdateTaskBeginDateModel model);

        /// <summary>
        /// Shift task model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ShiftTask(ShiftTaskModel model);
    }
}
