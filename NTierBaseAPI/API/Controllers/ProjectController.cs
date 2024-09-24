using Application.Models;
using Application.Models.Asset;
using Application.Models.Project;
using Application.Models.User;
using Application.Services;
using Application.Validators.Project;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Crmf;
using System.Security.AccessControl;

namespace API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]CreateProjectModel model)
        {
            return Ok(ApiResult<CreateProjectResponseModel>.Success(await _projectService.Create(model)));
        }

        [HttpPost]
        [Route("{projectId}/assets")]
        [Authorize]
        public async Task<IActionResult> AddProjectAsset([FromForm]AddProjectAssetModel model)
        {
            await _projectService.AddProjectAsset(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpGet]
        [Route("{projectId}/assets")]
        [Authorize]
        public async Task<IActionResult> GetAssetsOfProject(string projectId)
        {
            return Ok(ApiResult<List<ViewAssetModel>>.Success(await _projectService.GetAssetsOfProject(projectId)));
        }

        [HttpDelete]
        [Route("{projectId}/assets/{assetId}")]
        [Authorize]
        public async Task<IActionResult> RemoveAssetOfProject([FromRoute]RemoveAssetOfProjectModel model)
        {
            await _projectService.RemoveAssetOfProject(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(UpdateProjectModel model)
        {
            await _projectService.Update(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPost]
        [Route("{id}/members")]
        [Authorize]
        public async Task<IActionResult> AddProjectMemeber(AddProjectMemeberModel model)
        {
            await _projectService.AddMember(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpHead]
        [Route("{projectId}/members/{userId}")]
        public async Task<IActionResult> IsUserJoinToProject([FromRoute]IsUserJoinToProjectModel model)
        {
            await _projectService.IsUserJoinToProject(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPut]
        [Route("{id}/enddate")]
        [Authorize]
        public async Task<IActionResult> UpdateEndDate(UpdateProjectEndDateModel model)
        {
            await _projectService.UpdateEndDate(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPut]
        [Route("{id}/begindate")]
        [Authorize]
        public async Task<IActionResult> UpdateBeginDate(UpdateProjectBeginDateModel model)
        {
            await _projectService.UpdateBeginDate(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpGet]
        [Route("{id}/preview-before-update-begindate")]
        [Authorize]
        public async Task<IActionResult> GetPreviewBeforeUpdateBeginDate(PreviewBeforeUpdateProjectBeginDateModel model)
        {
            return Ok(ApiResult<PreviewBeforeUpdateProjectBeginDateResponseModel>
                .Success(await _projectService.GetPreviewBeforeUpdateBeginDate(model)));
        }

        [HttpGet]
        [Route("{id}/preview-before-update-enddate")]
        [Authorize]
        public async Task<IActionResult> GetPreviewBeforeUpdateEndDate(PreviewBeforeUpdateProjectEndDateModel model)
        {
            return Ok(ApiResult<PreviewBeforeUpdateProjectEndDateResponseModel>
                .Success(await _projectService.GetPreviewBeforeUpdateEndDate(model)));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(GetAllProjectModel model)
        {
            return Ok(ApiResult<GetAllProjectResponseModel>
                .Success(await _projectService.GetAll(model)));
        }

        [HttpGet]
        [Route("{id}/overview")]
        [Authorize]
        public async Task<IActionResult> GetProjectOverview(string id)
        {
            return Ok(ApiResult<ViewProjectOverviewModel>
                .Success(await _projectService.GetProjectOverview(id)));
        }

        [HttpGet]
        [Route("{id}/timeline")]
        [Authorize]
        public async Task<IActionResult> GetTimeline(ViewProjectTimeLineModel model)
        {
            return Ok(ApiResult<ViewProjectTimeLineResponseModel>
                .Success(await _projectService.GetProjectTimeline(model)));
        }

        [HttpGet]
        [Route("{id}/members")]
        [Authorize]
        public async Task<IActionResult> GetProjectMembers(string id)
        {
            return Ok(ApiResult<List<ViewProfileModel>>
                .Success(await _projectService.GetProjectMembers(id)));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProject(string id)
        {
            return Ok(ApiResult<ViewProjectModel>
                .Success(await _projectService.GetProject(id)));
        }

        [HttpHead]
        [Route("{id}/is-valid-before-update-begindate")]
        [Authorize]
        public async Task<IActionResult> CheckBeforeUpdateBeginDate(CheckBeforeUpdateBeginDateModel model)
        {
            await _projectService.CheckBeforeUpdateBeginDate(model);
            return Ok(ApiResult<object?>.Success(null));
        }
    }
}
