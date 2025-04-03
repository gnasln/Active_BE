using AutoMapper;
using Bff.Application.Tenants.Common;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Unit.common;
using PerfSvc.Infrastructure;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.UnitMember.Commands
{
    public record AddMemberToUnitCommand : IRequest<ResultCustom<string>>
    {
        public Guid MemberId { set; get; }

        public string? MemberName { set; get; } = string.Empty;

        public Guid UnitId { set; get; }

        public string? MemberFullName { set; get; } = string.Empty;

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<AddMemberToUnitCommand, Domain.Entities.UnitMember>();
            }
        }
    }


    public class AddMemberToUnitCommandHandler : IRequestHandler<AddMemberToUnitCommand, ResultCustom<string>>
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IUnitMemberRepository _unitMemberRepository;
        private readonly IUnitRepository _unitRepository;

        public AddMemberToUnitCommandHandler(IMapper mapper, ISender sender, IUnitMemberRepository unitMemberRepository, IUnitRepository unitRepository)
        {
            _mapper = mapper;
            _sender = sender;
            _unitMemberRepository = unitMemberRepository;
            _unitRepository = unitRepository;
        }
        
        public async Task<ResultCustom<string>> Handle(AddMemberToUnitCommand rq, CancellationToken cancellationToken)
        {
            try
            {
                // check unit
                var checkUnit = await _unitRepository.GetUnitById(rq.UnitId, cancellationToken);
                if (checkUnit == null)
                    return new ResultCustom<string>()
                    {
                        Status = StatusCode.NOTFOUND,
                        Message = new[] { "Unit doesn't exist" }
                    };

                // check owner of tenant and owner of unit
                CheckOwnerOfTenant o = new() { TenantId = checkUnit.TenantId };
                var checkIsOwner = await _sender.Send(o, cancellationToken);
                CheckManagerOfUnit m = new() { UnitId = rq.UnitId };

                var checkIsManager = await _sender.Send(m, cancellationToken);
                if (!checkIsOwner && !checkIsManager)
                    return new ResultCustom<string>
                    {
                        Status = StatusCode.FORBIDDEN,
                        Message = new[] { "Forbidden" }
                    };
                //check member exist
                var checkMemberExist = await _unitMemberRepository.GetUnitMemberById(rq.UnitId, rq.MemberId, cancellationToken);

                if (checkMemberExist != null)
                    return new ResultCustom<string>
                    {
                        Status = StatusCode.CONFLICT,
                        Message = new[] { "This user is already a member of the unit!" }
                    };

                var entity = _mapper.Map<Domain.Entities.UnitMember>(rq);
                await _unitMemberRepository.AddUnitMember(entity, cancellationToken);
                return new ResultCustom<string>
                {
                    Status = StatusCode.CREATED,
                    Message = new[] { $"Added Member to unit successfully!" },
                    Data = entity.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                return new ResultCustom<string>
                {
                    Status = StatusCode.INTERNALSERVERERROR,
                    Message = new[] { $"ERROR :: {ex}" }
                };
            }
        }
    }
}