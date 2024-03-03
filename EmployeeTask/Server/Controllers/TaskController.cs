namespace EmployeeTask.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService) 
        {
            _taskService = taskService;
        }
        [HttpPost]
        [Route("CreateTask")]
        public async Task<IActionResult> CreateTask(AssignedTaskModel model)
        {
            AssignedTask assignedTask = new AssignedTask
            {
                EmployeeId = model.EmployeeId,
                TaskName = model.TaskName,
                TaskStatus = model.TaskStatus
            };
            var result = await _taskService.AddTask(assignedTask);
            if (result.Status.Equals("Success"))
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpPost]
        [Route("UpdateTask")]
        public async Task<IActionResult> UpdateTask(AssignedTaskModel model)
        {
            AssignedTask assignedTask = new AssignedTask
            {
                EmployeeId = model.EmployeeId,
                TaskName = model.TaskName,
                TaskStatus = model.TaskStatus,
                Id = model.Id
            };
            var result = await _taskService.UpdateTask(assignedTask);
            if (result.Status.Equals("Success"))
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet]
        [Route("GetTasksForAdminEmployee/{id}")]
        public async Task<IActionResult> GetTasksForAdminEmployee(string id)
        {
            var result = await _taskService.GetTasksForAdminEmployee(Convert.ToInt32(id));
            return Ok(result);
        }

        [HttpGet]
        [Route("GetTasksByEmployeeId/{id}")]
        public async Task<IActionResult> GetTasksByEmployeeId(string id)
        {
            var result = await _taskService.GetTasksByEmployeeId(id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var result = await _taskService.DeleteTask(Convert.ToInt32(id));
            if (result.Status.Equals("Success"))
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
