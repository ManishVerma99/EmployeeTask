namespace EmployeeTaskHub.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AssignedTask> AssignedTasks { get; set; }
    }
}
