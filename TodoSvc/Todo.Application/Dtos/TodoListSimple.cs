using AutoMapper;
using TodoSvc.Domain.Entities;
using TodoSvc.Domain.Enums;

namespace TodoSvc.Application.Dtos;

public class TodoSimple
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public PriorityLevel Priority { get; init; }

    public TodoStatus Status { get; init; }
    public Guid? ObjectId { get; init; }
    public Guid? Owner { get; init; }
    public string OwnerName { get; init; } = "";
    public Guid? Assignee { get; init; }
    public string AssigneeName { get; init; } = "";
    public bool IsDone { get; set; }

    public DateTime? DueDate { get; init; } = null;
    public ICollection<TodoSimple>? SubTodoItems { get; init; }

    // AutoMapper
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TodoItem, TodoSimple>();
        }
    }
}
