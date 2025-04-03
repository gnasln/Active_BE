using Bff.Application.Tenants.Common;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Dtos;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.Unit.Queries
{
    public record ReadUnitQuery : IRequest<ResultCustom<UnitSimple>>
    {
        public required Guid Id { get; set; }
    }
    
    public class ReadUnitQueryHandler : IRequestHandler<ReadUnitQuery, ResultCustom<UnitSimple>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ISender _sender;
        private readonly IUnitMemberRepository _unitMemberRepository;

        public ReadUnitQueryHandler(IUnitRepository unitRepository, ISender sender, IUnitMemberRepository unitMemberRepository)
        {
            _unitRepository = unitRepository;
            _sender = sender;
            _unitMemberRepository = unitMemberRepository;
        }

        public async Task<ResultCustom<UnitSimple>> Handle(ReadUnitQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var unit = await _unitRepository.GetUnitById(request.Id, cancellationToken);
                var listMemberUnits = await _unitMemberRepository.GetAllUnitMember(request.Id, null, cancellationToken);

                // check tenant exist 
                CheckTenantExist t = new() { TenantId = unit.TenantId };
                var checkTenantExist = await _sender.Send(t, cancellationToken);
                if (!checkTenantExist) return new ResultCustom<UnitSimple>
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "This tenant is not exist, can't create unit" }
                };
                

                var memberUnits = listMemberUnits.Select(x => new { x.MemberId, x.MemberName }).ToList();
                var res = new UnitSimple()
                {
                    Id = unit.Id,
                    TenantId = unit.TenantId,
                    Name = unit.Name,
                    Description = unit.Description,
                    ManagerId = unit.ManagerId,
                    ManagerName = unit.ManagerName,
                    Priority = unit.Priority,
                    DueDate = unit.DueDate,
                    CreatedDate = unit.CreatedDate,
                    ParentUnitId = unit.ParentUnitId,
                    memberUnitIds = memberUnits.Select(x => x.MemberId).ToList(),
                    memberUnitNames = memberUnits.Select(x => x.MemberName).ToList()
                };

                return new ResultCustom<UnitSimple>
                {
                    Status = StatusCode.OK,
                    Message = new[] { "Read Unit Successfully" },
                    Data = res
                };
                
            }catch (Exception ex)
            {
                return new ResultCustom<UnitSimple>
                {
                    Status = StatusCode.INTERNALSERVERERROR,
                    Message = new[] { $"ERROR :: {ex}" }
                };
            }
        }
    }
}
