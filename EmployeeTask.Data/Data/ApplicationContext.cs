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
        public DbSet<Chat> Chats { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.FromUser)
                .WithMany()
                .HasForeignKey(c => c.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.ToUser)
                .WithMany()
                .HasForeignKey(c => c.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
