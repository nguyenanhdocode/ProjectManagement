using Application.Models;
using Application.Models.Dashboard;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Route("today-tasks")]
        [Authorize]
        public async Task<IActionResult> ViewTodayTasks(ViewTodayTasksModel model)
        {
            var results = await _dashboardService.GetTodayTasks(model);
            return Ok(ApiResult<List<ViewTodayTasksItem>>.Success(results));
        }

        [HttpGet]
        [Route("late-tasks")]
        [Authorize]
        public async Task<IActionResult> ViewLateTasks(ViewLateTasksModel model)
        {
            var results = await _dashboardService.GetLateTasks(model);
            return Ok(ApiResult<List<ViewLateTasksItem>>.Success(results));
        }
    }
}
