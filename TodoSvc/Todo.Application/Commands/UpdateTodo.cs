using Ardalis.GuardClauses;
using MediatR;
using Todo.Domain.Entities;
using TodoSvc.Domain.Entities;
using TodoSvc.Domain.Enums;

namespace TodoSvc.Application.Commands;

public record UpdateTodoRequest : IRequest
{
    public required Guid Id { get; init; }
    public string? Title { get; init; }

    public string? Description { get; init; }

    public PriorityLevel? Priority { get; init; }

    public TodoStatus? Status { get; init; }

    public DateTime? ModifiedDate { get; init; }

    public DateTime? DueDate { get; init; }

    #region outside of TodoService (come from Performance Management Service)
    public Guid? Owner { get; init; }
    public string? OwnerName { get; init; }
    public Guid? Assigner { get; init; }
    public Guid? Assignee { get; init; }
    public string? AssigneeName { get; init; }
    public Guid? ObjectId { get; init; }
    public bool IsDone { get; set; } = false;
    #endregion
    public List<Guid>? MemberIds { get; set; }
    public List<string>? MemberName { get; set; }
}

public class UpdateTodoRequestHandler(ITodoDbContext db) : IRequestHandler<UpdateTodoRequest>
{
    private readonly ITodoDbContext _context = db;


    public async Task Handle(UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems.FindAsync([request.Id], cancellationToken);
        Guard.Against.NotFound(request.Id, entity);



        if (request.Title is not null) entity.Title = request.Title;
        if (request.Description is not null) entity.Description = request.Description;

        if (request.Priority is not null) entity.Priority = (PriorityLevel)request.Priority;
        if (request.Status is not null) entity.Status = (TodoStatus)request.Status;
        entity.ModifiedDate = request.ModifiedDate;
        if (request.DueDate is not null) entity.DueDate = request.DueDate;
        if (request.Owner is not null) entity.Owner = request.Owner.GetValueOrDefault();
        if (request.OwnerName is not null) entity.OwnerName = request.OwnerName;
        if (request.Assigner is not null) entity.Assigner = request.Assigner.GetValueOrDefault();
        if (request.Assignee is not null) entity.Assignee = request.Assignee;
        if (request.AssigneeName is not null) entity.AssigneeName = request.AssigneeName;
        if (request.ObjectId is not null) entity.ObjectId = request.ObjectId;
        // Update TodoMembers
        if (request.MemberIds is not null && request.MemberName is not null && request.MemberIds.Count == request.MemberName.Count)
        {
            // Remove old members associated with this TodoItem
            var existingMembers = _context.TodoMembers.Where(x => x.TodoId == entity.Id).ToList();
            if (existingMembers.Any())
            {
                _context.TodoMembers.RemoveRange(existingMembers);
                await _context.SaveChangeAsync(cancellationToken); // Save changes after removal
            }

            // Add new members
            var newMembers = request.MemberIds.Select((memberId, index) => new TodoMembers
            {
                TodoId = entity.Id,
                MemberId = memberId,
                MemberName = request.MemberName[index]
            }).ToList();

            await _context.TodoMembers.AddRangeAsync(newMembers, cancellationToken);
        }
        var history = new TodoHistory
        {
            Id = Guid.NewGuid(),
            TodoId = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority.ToString(),
            DueDate = entity.DueDate,
            ModifiedDate = DateTime.UtcNow.AddHours(7),
        };

        await _context.TodoHistories.AddAsync(history, cancellationToken);
        await _context.SaveChangeAsync(cancellationToken);
    }
}