namespace EmployeeTask.Service.Services
{
    public class TaskService:ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<Response> AddTask(AssignedTask model)
        {
            return await _taskRepository.AddTask(model);
        }

        public async Task<List<AssignedTask>> GetTasksForAdminEmployee(int id)
        {
            return await _taskRepository.GetTasksForAdminEmployee(id);
        }

        public async Task<List<AssignedTask>> GetTasksByEmployeeId(string id)
        {
            return await _taskRepository.GetTasksByEmployeeId(id);
        }

        public async Task<Response> UpdateTask(AssignedTask model)
        {
           return await _taskRepository.UpdateTask(model);
        }

        public async Task<Response> DeleteTask(int id)
        {
            return await _taskRepository.DeleteTask(id);
        }

    }
}
