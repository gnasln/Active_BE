using Bff.Application.Tenants.Common;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.Unit.Commands
{
    public record DeleteUnitCommand : IRequest<ResultCustom<string>>
    {
        public required Guid Id { get; set; }
    }

    public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, ResultCustom<string>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ISender _sender;

        public DeleteUnitCommandHandler(IUnitRepository unitRepository, ISender sender, IPerfDbContext db)
        {
            _unitRepository = unitRepository;
            _sender = sender;
            
        }

        public async Task<ResultCustom<string>> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var unit = await _unitRepository.GetUnitById(request.Id, cancellationToken);

                if(unit == null)
                {
                    return new ResultCustom<string>
                    {
                        Status = StatusCode.NOTFOUND,
                        Message = new[] { "Unit not found" }
                    };
                }

                // check tenant exist 
                CheckTenantExist t = new() { TenantId = unit.TenantId };
                var checkTenantExist = await _sender.Send(t);
                if (!checkTenantExist) return new ResultCustom<string>
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "This tenant is not exist, can't create unit" }
                };

                await _unitRepository.DeleteUnit(request.Id, cancellationToken);
                return new ResultCustom<string>
                {
                     Status = StatusCode.OK,
                     Message = new[] { "Delete unit successfully" }
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
