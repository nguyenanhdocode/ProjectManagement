using Application.Common.Tree;
using Application.Models;
using Application.Models.Task;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]CreateTaskModel model)
        {
            return Ok(ApiResult<CreateTaskResponseModel>.Success(await _taskService.Create(model)));
        }

        [HttpGet]
        [Route("{id}/preview-before-update-enddate")]
        [Authorize]
        public async Task<IActionResult> GetPreviewBeforeUpdateDateTime(
            PreviewBeforeUpdateEndDateModel model)
        {
            return Ok(ApiResult<PreviewBeforeUpdateEndDateResponseModel>
                .Success(await _taskService.GetPreviewBeforeUpdateEndDate(model)));
        }
    }
}
