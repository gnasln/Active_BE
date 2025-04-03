using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoSvc.Application.Dtos;

namespace TodoSvc.Application.Queries;

public record GetTodoDetailRequest : IRequest<TodoDetail?>
{
    public required Guid Id { get; init; }
}

public class GetTodoDetailRequestHandler(ITodoDbContext db, IMapper mapper) : IRequestHandler<GetTodoDetailRequest, TodoDetail?>
{
    private readonly ITodoDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<TodoDetail?> Handle(GetTodoDetailRequest request, CancellationToken cancellationToken)
    {
        return await _db.TodoItems.Where(x => x.Id == request.Id)
            .ProjectTo<TodoDetail>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}