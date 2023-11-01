using Entities.Enum;

namespace Api.Models
{
    public class TasksAddViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public string UserId { get; set; }
    }
}
