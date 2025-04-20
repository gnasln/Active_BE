using AutoMapper;
using TodoSvc.Domain.Entities;
using TodoSvc.Domain.Enums;

namespace TodoSvc.Application.Dtos;

public class TodoDetail
{
    public Guid Id { get; init; }
    public required string Owner { get; init; }
    public required string OwnerName { get; init; }
    public DateTime CreateDate { get; init; }
    public DateTime? DueDate { get; init; }
    public required string Title { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    public PriorityLevel? Priority { get; init; }
    public TodoStatus? Status { get; init; }
    public Guid? ParentTodoItemId { get; set; }
    public ICollection<TodoItem>? SubTodoItems { get; set; }
    public ICollection<TodoItemComment>? Comments { get; set; }
    public Guid? ObjectId { get; set; }
    public Guid? Assignee { get; set; }
    public string? AssigneeName { get; set; } 
    public bool IsDone { get; set; }
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TodoItem, TodoDetail>();
        }
    }
}
