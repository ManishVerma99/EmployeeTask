namespace EmployeeTask.Core.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey(nameof(FromUserId))]
        public virtual ApplicationUser FromUser { get; set; }
        [ForeignKey(nameof(ToUserId))]
        public virtual ApplicationUser ToUser { get; set; }
    }
}
