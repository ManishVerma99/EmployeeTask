namespace EmployeeTask.Data.IRepository
{
    public interface ITaskRepository
    {
        Task<Response> AddTask(AssignedTask model);
        Task<Response> UpdateTask(AssignedTask model);
        Task<List<AssignedTask>> GetTasksForAdminEmployee(int id);
        Task<List<AssignedTask>> GetTasksByEmployeeId(string userId);
        Task<Response> DeleteTask(int id);
    }
}
