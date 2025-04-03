using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using NetHelper.Common.Mappings;
using NetHelper.Common.Models;
using PerfSvc.Application.Dtos;
using PerfSvc.Infrastructure;

namespace PerfSvc.Application.KeyResult.Queries
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.KeyresultHistory, KeyResultHistoryDto>();
        }
    }

    public record GetAllKeyResultHisroryRequest : IRequest<PaginatedList<KeyResultHistoryDto>>
    {
        public Guid KeyResultId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
    public class GetKeyResultHistoryHandle : IRequestHandler<GetAllKeyResultHisroryRequest, PaginatedList<KeyResultHistoryDto>>
    {
        private readonly IPerfDbContext _DbContext;
        private IMapper _mapper;

        public GetKeyResultHistoryHandle(IPerfDbContext dbContext, IMapper mapper)
        {
            _DbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PaginatedList<KeyResultHistoryDto>> Handle(GetAllKeyResultHisroryRequest request, CancellationToken cancellationToken)
        {
            var keyResultHistory = await _DbContext.KeyresultHistories
            .Where(o => o.keyresultid == request.KeyResultId)
            .ProjectTo<KeyResultHistoryDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
            return keyResultHistory;
        }
    }
}