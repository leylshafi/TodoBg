namespace TodoWebService.Models.DTOs.Todo
{
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Email { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
