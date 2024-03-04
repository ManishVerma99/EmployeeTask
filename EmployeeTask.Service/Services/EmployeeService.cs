namespace EmployeeTask.Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(IEmployeeRepository employeeRepository, UserManager<ApplicationUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
        }

        public async Task<Response> CreateEmployee(RegisterModel model)
        {
            return await _employeeRepository.CreateEmployee(model);
        }

        public async Task<List<RegisterModel>> GetEmployees()
        {
            return await _employeeRepository.GetEmployees();
        }

        public async Task<Response> UpdateEmployee(UpdateEmployee model)
        {
            return await _employeeRepository.UpdateEmployee(model);
        }

        public async Task<Response> DeleteEmployee(string id)
        {
            return await _employeeRepository.DeleteEmployee(id);
        }

        public async Task<Employee> GetEmployee(string id)
        {
            return await _employeeRepository.GetEmployee(id);
        }

        public async Task<List<ApplicationUser>> GetUsers()
        {
            return await _employeeRepository.GetUsers();
        }
    }
}
