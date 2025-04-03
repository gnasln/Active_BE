using AutoMapper;
using Bff.Application.Tenants.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using PerfSvc.Application.Unit.common;
using PerfSvc.Infrastructure;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.UnitMember.Queries;

public class GetListUnitMemberQuery : IRequest<ResultCustom<List<Domain.Entities.UnitMember>>>
{
    public required Guid UnitId { get; set; }
    public string? MemberName { get; set; }
}

public class GetListUnitMemberQueryHandler : IRequestHandler<GetListUnitMemberQuery, ResultCustom<List<Domain.Entities.UnitMember>>>
{
    private readonly ISender _sender;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitMemberRepository _unitMemberRepository;


    public GetListUnitMemberQueryHandler(ISender sender, IUnitMemberRepository unitMemberRepository, IUnitRepository unitRepository)
    {
        _unitMemberRepository = unitMemberRepository;
        _unitRepository = unitRepository;
        _sender = sender;
    }

    public async Task<ResultCustom<List<Domain.Entities.UnitMember>>> Handle(GetListUnitMemberQuery rq,
        CancellationToken cancellationToken)
    {
        try
        {
            // check unit
            var checkUnit = await _unitRepository.GetUnitById(rq.UnitId, cancellationToken);
            if (checkUnit == null)
                return new ResultCustom<List<Domain.Entities.UnitMember>>()
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "Unit doesn't exist" }
                };

            var res = await _unitMemberRepository.GetAllUnitMember(rq.UnitId, rq.MemberName, cancellationToken);
            return new ResultCustom<List<Domain.Entities.UnitMember>>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Get Member of Unit successfully" },
                Data = res
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<List<Domain.Entities.UnitMember>>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"ERROR :: {ex}" }
            };
        }
    }
}