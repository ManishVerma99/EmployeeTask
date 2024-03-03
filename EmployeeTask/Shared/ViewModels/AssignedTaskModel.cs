namespace EmployeeTask.Shared.ViewModels
{
    public class AssignedTaskModel
    {
        public int Id { get; set; }
        [Required]
        public string TaskName { get; set; }
        public int EmployeeId { get; set; }
        public bool TaskStatus { get; set; }
    }
}
