using PerfSvc.Domain.Entities;

namespace PerfSvc.Infrastructure.Interface.Repository;

public interface IUnitRepository
{
    public Task<Unit> CreateUnit(Unit unit, CancellationToken cancellationToken);
    public Task<List<Unit>> GetAllUnitFromTenant(Guid tenantId, CancellationToken cancellationToken);
    public Task DeleteUnit(Guid id, CancellationToken cancellationToken);
    public Task<Unit> UpdateUnit(Unit unit, CancellationToken cancellationToken);
    public Task<Unit> GetUnitById(Guid id, CancellationToken cancellationToken);
    public Task<List<Unit>> GetUnitByParentId(Guid parentId, CancellationToken cancellationToken);
}