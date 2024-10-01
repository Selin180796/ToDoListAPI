namespace ToDoListAPI.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Description { get; set; }
    }
}

