namespace EmployeeTask.Data.IRepository
{
    public interface IEmployeeRepository
    {
        Task<Response> CreateEmployee(RegisterModel model);
        Task<List<RegisterModel>> GetEmployees();
        Task<Response> UpdateEmployee(UpdateEmployee model);
        Task<Response> DeleteEmployee(string id);
        Task<Employee> GetEmployee(string id);
    }
}
