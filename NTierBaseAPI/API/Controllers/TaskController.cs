using Application.Common;
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
        [Route("{id}/subtasks")]
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

        [HttpGet]
        [Route("{id}/overview")]
        [Authorize]
        public async Task<IActionResult> GetOverview(ViewTaskOverviewModel model)
        {
            return Ok(ApiResult<ViewTaskOverviewResponseModel>.Success(await _taskService.GetOvervew(model)));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(ApiResult<ViewTaskModel>.Success(await _taskService.GetTask(id)));
        }

        [HttpHead]
        [Route("{id}/is-affect-when-update-enddate")]
        [Authorize]
        public async Task<IActionResult> CheckAffectBeforeUpdateEndDate(CheckAffectBeforeUpdateEndDateModel model)
        {
            await _taskService.CheckAffectBeforeUpdateEndDate(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpGet]
        [Route("{id}/allowed-new-status")]
        [Authorize]
        public async Task<IActionResult> GetAllowedNewStatus(ViewAllowedNewTaskStatusModel model)
        {
            return Ok(ApiResult<List<AppTaskStatus>>.Success(await _taskService.GetAllowedNewStatus(model)));
        }

        [HttpGet]
        [Route("{id}/subtasks")]
        [Authorize]
        public async Task<IActionResult> GetSubtasks(ViewSubtasksModel model)
        {
            return Ok(ApiResult<List<ViewTaskModel>>.Success(await _taskService.GetSubtasks(model)));
        }

        [HttpPut]
        [Route("{id}/subtasks")]
        [Authorize]
        public async Task<IActionResult> UpdateSubtask(UpdateSubtaskModel model)
        {
            await _taskService.UpdateSubtask(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpHead]
        [Route("{id}/can-update-enddate")]
        [Authorize]
        public async Task<IActionResult> CheckCanUpdateBeginDate(CheckCanUpdateTaskBeginDateModel model)
        {
            await _taskService.CheckCanUpdateEndDate(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPut]
        [Route("{id}/time")]
        [Authorize]
        public async Task<IActionResult> Shift(ShiftTaskModel model)
        {
            await _taskService.ShiftTask(model);
            return Ok(ApiResult<object?>.Success(null));
        }
    }
}
