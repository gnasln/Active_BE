using Bff.Application.Tenants.Common;
using MediatR;
using PerfSvc.Application.Unit.common;

namespace PerfSvc.Application.Object.Common
{
    public class AuthorizationChecker
    {
        private readonly ISender _sender;

        public AuthorizationChecker(ISender sender)
        {
            _sender = sender;
        }

        public async Task<bool> IsManagerOfUnit(Guid unitId, CancellationToken cancellationToken)
        {
            CheckManagerOfUnit checkManagerOfUnit = new() { UnitId = unitId };
            return await _sender.Send(checkManagerOfUnit, cancellationToken);
        }

        public async Task<bool> IsOwnerOfTenant(Guid tenantId, CancellationToken cancellationToken)
        {
            CheckOwnerOfTenant checkOwnerOfTenant = new() { TenantId = tenantId };
            return await _sender.Send(checkOwnerOfTenant, cancellationToken);
        }

        public async Task<bool> IsAuthorized(Guid unitId, Guid tenantId, CancellationToken cancellationToken)
        {
            var isManager = await IsManagerOfUnit(unitId, cancellationToken);
            var isOwner = await IsOwnerOfTenant(tenantId, cancellationToken);
            return isManager || isOwner;
        }
    }
}