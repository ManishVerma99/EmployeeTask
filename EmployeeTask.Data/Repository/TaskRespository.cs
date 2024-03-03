namespace EmployeeTask.Data.Repository
{
    public class TaskRespository : ITaskRepository
    {
        private readonly ApplicationContext _applicationContext;
        public TaskRespository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }
        public async Task<Response> AddTask(AssignedTask model)
        {
            try
            {
                _applicationContext.AssignedTasks.Add(model);
                await _applicationContext.SaveChangesAsync();
                return new Response { Message = "Task created successfully", Status = "Success" };
            }
            catch (Exception ex)
            {
                return new Response { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<Response> UpdateTask(AssignedTask model)
        {
            try
            {
                var task = await _applicationContext.AssignedTasks.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
                if (task == null)
                {
                    return new Response { Status = "Error", Message = "Task not exist" };
                }
                task.TaskName = model.TaskName;
                task.TaskStatus = model.TaskStatus;
                _applicationContext.AssignedTasks.Update(task);
                await _applicationContext.SaveChangesAsync();
                return new Response { Message = "Task updated successfully", Status = "Success" };
            }
            catch (Exception ex)
            {
                return new Response { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<List<AssignedTask>> GetTasksForAdminEmployee(int id)
        {
            var result = await _applicationContext.AssignedTasks.Where(x => x.EmployeeId == id).ToListAsync();
            return result ?? new List<AssignedTask>();
        }

        public async Task<List<AssignedTask>> GetTasksByEmployeeId(string userId)
        {
            var employee = await _applicationContext.Employees.FirstOrDefaultAsync(x => x.UserId == userId);
            var result = await _applicationContext.AssignedTasks.Where(x => x.EmployeeId == employee.Id).ToListAsync();
            return result ?? new List<AssignedTask>();
        }

        public async Task<Response> DeleteTask(int id)
        {
            try
            {
                var userExists = await _applicationContext.AssignedTasks.FirstOrDefaultAsync(x => x.Id == id);
                if (userExists == null)
                {
                    return new Response { Status = "Error", Message = "User not exists!" };
                }
                _applicationContext.AssignedTasks.Remove(userExists);
                await _applicationContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "Task deleted successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Status = "Error", Message = ex.Message };
            }
        }
    }
}
