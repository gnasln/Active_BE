namespace TodoSvc.Domain.Entities;

public class TodoItemComment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Comment { get; set; }

    public Guid? ParentTodoItemCommentId { get; set; }
    public TodoItemComment? ParentTodoItemComment { get; set; }

    [ForeignKey(nameof(TodoItem))]
    public Guid TodoItemId { get; set; }
}