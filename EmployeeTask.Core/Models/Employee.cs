namespace EmployeeTask.Core.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string? CreatedBy { get; set; }
        public string? Password { get; set; }
    }
}
