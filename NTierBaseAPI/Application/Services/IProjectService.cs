using Application.Models.Asset;
using Application.Models.Project;
using Application.Models.Task;
using Application.Validators.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.User;

namespace Application.Services
{
    public interface IProjectService
    {
        /// <summary>
        /// Create project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CreateProjectResponseModel> Create(CreateProjectModel model);

        /// <summary>
        /// Add asset to project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AddProjectAsset(AddProjectAssetModel model);

        /// <summary>
        /// Get assets of project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<List<ViewAssetModel>> GetAssetsOfProject(string projectId);

        /// <summary>
        /// Remove asset of project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task RemoveAssetOfProject(RemoveAssetOfProjectModel model);

        /// <summary>
        /// Update project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task Update(UpdateProjectModel model);

        /// <summary>
        /// Add member to project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AddMember(AddProjectMemeberModel model);

        /// <summary>
        /// Check if is user join to project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task IsUserJoinToProject(IsUserJoinToProjectModel model);

        /// <summary>
        /// Update project enddate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateEndDate(UpdateProjectEndDateModel model);

        /// <summary>
        /// Update project begindate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateBeginDate(UpdateProjectBeginDateModel model);

        /// <summary>
        /// Get preview before update project begindate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<PreviewBeforeUpdateProjectBeginDateResponseModel> GetPreviewBeforeUpdateBeginDate(
            PreviewBeforeUpdateProjectBeginDateModel model);

        /// <summary>
        /// Get preview before update project enddate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<PreviewBeforeUpdateProjectEndDateResponseModel> GetPreviewBeforeUpdateEndDate(
            PreviewBeforeUpdateProjectEndDateModel model);

        /// <summary>
        /// Get all project with filter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<GetAllProjectResponseModel> GetAll(ViewAllProjectModel model);

        /// <summary>
        /// Get project detail
        /// </summary>
        /// <returns></returns>
        Task<ViewProjectOverviewModel> GetProjectOverview(string id);

        /// <summary>
        /// Get project timeline
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ViewProjectTimeLineResponseModel> GetProjectTimeline(ViewProjectTimeLineModel model);

        /// <summary>
        /// Get project members
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<ViewProfileModel>> GetProjectMembers(string id);

        /// <summary>
        /// Get project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ViewProjectModel> GetProject(string id);

        /// <summary>
        /// Check valid before update begindate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CheckBeforeUpdateBeginDate(CheckBeforeUpdateBeginDateModel model);

        /// <summary>
        /// Check valid before update enddate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CheckBeforeUpdateEndDate(Models.Project.CheckBeforeUpdateEndDateModel model);

        /// <summary>
        /// Get tasks of project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<ViewTaskModel>> GetTasksOfProject(ViewProjectTasksModel model);

        /// <summary>
        /// Delete project members
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task RemoveMembers(DeleteProjectMembersModel model);

        /// <summary>
        /// Delete project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task Delete(DeleteProjectModel model);

        /// <summary>
        /// Get avaiable previous task
        /// </summary>
        /// <returns></returns>
        Task<List<ViewTaskModel>> GetAvaiablePrevTasks(ViewAvaiablePrevTasksModel model);

        /// <summary>
        /// View project progress
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<double> GetProjectProgress(ViewProjectProgressModel model);
    }
}
