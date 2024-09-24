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
        public async Task<IActionResult> Create([FromBody] CreateTaskModel model)
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

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(UpdateTaskModel model)
        {
            await _taskService.Update(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            await _taskService.Delete(id);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPost]
        [Route("{id}/sub-tasks")]
        [Authorize]
        public async Task<IActionResult> CreateSubTask(CreateSubtaskModel model)
        {
            return Ok(ApiResult<CreateSubtaskReponseModel>.Success(await _taskService.CreateSubTask(model)));
        }

        [HttpPut]
        [Route("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(UpdateTaskStatusModel model)
        {
            await _taskService.UpdateStatus(model);
            return Ok(ApiResult<object?>.Success(null));
        }
    }
}
