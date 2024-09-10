using Application.Models;
using Application.Models.Asset;
using Application.Models.Project;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create(CreateProjectModel model)
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
    }
}
