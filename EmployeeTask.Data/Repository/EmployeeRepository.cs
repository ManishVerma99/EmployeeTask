namespace EmployeeTask.Data.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationContext _applicationContext;
        public EmployeeRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }
        public async Task<Response> CreateEmployee(RegisterModel model)
        {
            try
            {
                var result = await _applicationContext.Users.FirstOrDefaultAsync(x => x.UserName == model.Username);
                if (result == null)
                {
                    return new Response { Message = "User not created", Status = "Error" };
                }
                await _applicationContext.Employees.AddAsync(new Employee { UserId = result.Id,CreatedBy=model.CreatedBy, Password = model.Password });
                await _applicationContext.SaveChangesAsync();
                return new Response { Message = "User Created", Status = "Sucess" };
            }
            catch (Exception ex)
            {
                return new Response { Message = ex.Message };
            }
        }

        public async Task<List<RegisterModel>> GetEmployees()
        {
            var employees = await (
                from emp in _applicationContext.Employees
                join user in _applicationContext.Users on emp.UserId equals user.Id
                select new RegisterModel
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.UserName,
                    Id = user.Id,
                    Password = emp.Password
                }
            ).ToListAsync();

            return employees;
        }

        public async Task<Response> UpdateEmployee(UpdateEmployee model)
        {
            try
            {
                var userExists = await _applicationContext.Users.FirstOrDefaultAsync(x => x.UserName == model.Username);
                if (userExists == null)
                {
                    return new Response { Status = "Error", Message = "User not exists!" };
                }
                userExists.Email = model.Email;
                userExists.UserName = model.Username;
                userExists.FirstName = model.FirstName;
                userExists.LastName = model.LastName;
                _applicationContext.Users.Update(userExists);
                await _applicationContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "User updated successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<Response> DeleteEmployee(string id)
        {
            try
            {
                var userExists = await _applicationContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (userExists == null)
                {
                    return new Response { Status = "Error", Message = "User not exists!" };
                }
                _applicationContext.Users.Remove(userExists);
                await _applicationContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "User deleted successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<Employee> GetEmployee(string id)
        {
            var result = _applicationContext.Employees.FirstOrDefault(x=>x.UserId == id);
            if(result == null)
            {
                return null;
            }
            return result;
        }

        public async Task<List<ApplicationUser>> GetUsers()
        {
            return await _applicationContext.Users.ToListAsync();
        }
    }
}
