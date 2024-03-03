namespace EmployeeTask.Core.Models
{
    public class AssignedTask
    {
        [Key]
        public int Id { get; set; }
        public string? TaskName { get; set; }
        public bool TaskStatus { get; set; }
        public int EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }
    }
}
