using Todo.Domain.Entities;

namespace TodoSvc.Domain.Entities;

public class TodoItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public required string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public PriorityLevel Priority { get; set; }

    public TodoStatus Status { get; set; } = TodoStatus.New;

    public DateTime? CreatedDate { get; set;} = DateTime.Now;

    public DateTime? ModifiedDate { get; set;} = DateTime.Now;

    public DateTime? DueDate { get; set; } = null;
    public DateTime? CompleteDate { get; set; } = null;

    #region outside of TodoService (come from Performance Management Service)
    public required Guid Owner { get; set; }
    public required string OwnerName { get; set; }
    public Guid Assigner { get; set; } = Guid.Empty;
    public Guid? Assignee { get; set; } = Guid.Empty;
    public string AssigneeName { get; set; } = string.Empty;
    public Guid? UnitId { get; set; } = Guid.Empty;
    public Guid? ModifiedBy { get; set; } = Guid.Empty;
    #endregion


    [ForeignKey(nameof(TodoItem))]
    public Guid? ParentTodoItemId { get; set; }

    public TodoItem? ParentTodoItem { get; set; } = null!;
    public ICollection<TodoItem>? SubTodoItems { get; set;}
    public ICollection<TodoItemComment>? Comments { get; set; }
}
