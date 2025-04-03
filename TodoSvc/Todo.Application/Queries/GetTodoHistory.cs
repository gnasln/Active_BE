using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Mappings;
using NetHelper.Common.Models;
using TodoSvc.Application;
using TodoSvc.Application.Dtos;
using TodoSvc.Domain.Entities;

namespace Todo.Application.Queries
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map TodoHistory to TodoHistorysDTO
            CreateMap<TodoHistory, TodoHistorysDTO>();
        }
    }
    public record GetTodoHistoryRequest : IRequest<PaginatedList<TodoHistorysDTO>>
    {
        public Guid TodoId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }

    public class GetTodoHistoryDetailRequestHandler : IRequestHandler<GetTodoHistoryRequest, PaginatedList<TodoHistorysDTO>>
    {
        private readonly ITodoDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetTodoHistoryDetailRequestHandler(ITodoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PaginatedList<TodoHistorysDTO>> Handle(GetTodoHistoryRequest request, CancellationToken cancellationToken)
        {
            var todoHistories = await _dbContext.TodoHistories
                .Where(x => x.TodoId == request.TodoId)
                .ProjectTo<TodoHistorysDTO>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
            return todoHistories;
        }
    }

}
