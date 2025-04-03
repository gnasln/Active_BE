using AutoMapper;
using MediatR;
using Todo.Application.Dtos;
using Todo.Domain.Entities;
using TodoSvc.Application.Dtos;
using TodoSvc.Domain.Entities;
using TodoSvc.Domain.Enums;

namespace TodoSvc.Application.Commands;

public record CreateTodoCommand : IRequest<TodoMemberDto>
{
    public required string Title { get; init; }

    public string? Description { get; init; } = string.Empty;

    public PriorityLevel Priority { get; init; } = PriorityLevel.None;

    public TodoStatus Status { get; init; } = TodoStatus.New;

    public DateTime? CreatedDate { get; init; } = DateTime.Now;

    public DateTime? ModifiedDate { get; init; } = DateTime.Now;

    public DateTime? DueDate { get; init; } = null;

    #region outside of TodoService (come from Performance Management Service)
    public required Guid Owner { get; init; }
    public required string OwnerName { get; init; }
    public Guid Assigner { get; init; } = Guid.Empty;
    public Guid? Assignee { get; init; } = Guid.Empty;
    public string AssigneeName { get; init; } = string.Empty;
    public Guid? UnitId { get; init; } = Guid.Empty;
    #endregion
    public Guid? ParentTodoItemId { get; set; }
    public List<Guid>? MemberIds { get; set; }
    public List<string>? MemberName { get; set; }
    #region automapper
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateTodoCommand, TodoItem>();
            CreateMap<TodoItem, TodoMemberDto>();
        }
    }
    #endregion
}

public class CreateTodoCommandHandler(ITodoDbContext dbContext, IMapper mapper) : IRequestHandler<CreateTodoCommand, TodoMemberDto>
{
    private readonly ITodoDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<TodoMemberDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<CreateTodoCommand, TodoItem>(request);
        await _dbContext.TodoItems.AddAsync(entity, cancellationToken);

        List<TodoMembers>? members = null;

        if (request.MemberIds != null && request.MemberName != null && request.MemberIds.Count == request.MemberName.Count)
        {
            members = request.MemberIds.Select((t, i) => new TodoMembers
            {
                TodoId = entity.Id,
                MemberId = request.MemberIds[i],
                MemberName = request.MemberName[i]
            }).ToList();

            await _dbContext.TodoMembers.AddRangeAsync(members, cancellationToken);
        }

        await _dbContext.SaveChangeAsync(cancellationToken);

        // Manually set the member IDs and names in the DTO after they are saved
        var resultDto = _mapper.Map<TodoItem, TodoMemberDto>(entity);

        if (members != null)
        {
            resultDto.MemberIds = members.Select(m => m.MemberId).ToList();
            resultDto.MemberName = members.Select(m => m.MemberName).ToList();
        }
        return resultDto;
    }
}