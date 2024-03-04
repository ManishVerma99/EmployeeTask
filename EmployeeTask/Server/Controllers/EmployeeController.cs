using Microsoft.AspNetCore.Authorization;

namespace EmployeeTask.Server.Controllers
{
 
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        { 
            _employeeService = employeeService;
        }

        [HttpGet]
        [Route("GetEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await _employeeService.GetEmployees();
            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status204NoContent, result);
        }

        [HttpPost]
        [Route("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployee model)
        {
            var result = await _employeeService.UpdateEmployee(model);
            if (result.Status.Equals("Success"))
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpDelete]
        [Route("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var result = await _employeeService.DeleteEmployee(id);
            if (result.Status.Equals("Success"))
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet]
        [Route("GetEmployee/{id}")]
        public async Task<IActionResult> GetEmployee(string id)
        {
            var result = await _employeeService.GetEmployee(id);
            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status204NoContent, result);
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _employeeService.GetUsers();
            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status204NoContent, result);
        }
    }
}
