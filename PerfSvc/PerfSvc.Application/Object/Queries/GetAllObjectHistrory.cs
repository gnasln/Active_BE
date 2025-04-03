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

namespace PerfSvc.Application.Object.Queries
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.ObjectHistory, ObjectHistoryDto>();
        }
    }

    public record GetObjectHistoryRequest : IRequest<PaginatedList<ObjectHistoryDto>>
    {

        public Guid ObjectId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class GetObjectHistroryDetailsHandle : IRequestHandler<GetObjectHistoryRequest, PaginatedList<ObjectHistoryDto>> {
        private readonly IPerfDbContext _DbContext;
        private readonly IMapper _mapper;
        public GetObjectHistroryDetailsHandle(IPerfDbContext dbContext, IMapper mapper) {
            _DbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<PaginatedList<ObjectHistoryDto>> Handle(GetObjectHistoryRequest request, CancellationToken cancellationToken) {
            var ObjectHistory = await _DbContext.objectHistories
            .Where(o => o.objecttbid == request.ObjectId)
            .ProjectTo<ObjectHistoryDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
            return ObjectHistory;
        }

    }
}