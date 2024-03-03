namespace EmployeeTask.Core.Interface
{
    public interface IEmployeeService
    {
        Task<Response> CreateEmployee(RegisterModel model);
        Task<List<RegisterModel>> GetEmployees();
        Task<Response> UpdateEmployee(UpdateEmployee model);
        Task<Response> DeleteEmployee(string id);
        Task<Employee> GetEmployee(string id);
    }
}
