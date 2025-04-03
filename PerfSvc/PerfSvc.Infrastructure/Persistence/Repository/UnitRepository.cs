using Microsoft.EntityFrameworkCore;
using PerfSvc.Domain.Entities;
using PerfSvc.Infrastructure.Interface.Repository;
using PrefSvc.Domain.Enums;

namespace PerfSvc.Infrastructure.Persistence.Repository;

public class UnitRepository : IUnitRepository
{
    private readonly IPerfDbContext _db;

    public UnitRepository(IPerfDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> CreateUnit(Unit unit, CancellationToken cancellationToken)
    {
        await _db.Units.AddAsync(unit, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return unit;
    }

    public async Task DeleteUnit(Guid id, CancellationToken cancellationToken)
    {
        var unit = await _db.Units.FindAsync(id, cancellationToken);
        _db.Units.Remove(unit);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Unit>> GetAllUnitFromTenant(Guid tenantId, CancellationToken cancellationToken)
    {
        var allUnits = await _db.Units
            .Where(u => u.TenantId == tenantId && u.ParentUnitId == null)
            .ToListAsync(cancellationToken);
        return allUnits;
    }

    public async Task<Unit> UpdateUnit(Unit unit, CancellationToken cancellationToken)
    {
        var existingUnit = await _db.Units.FindAsync(unit.Id , cancellationToken);
        if (existingUnit == null)
        {
            throw new KeyNotFoundException("Unit not found");
        }

        existingUnit.Name = unit.Name ?? existingUnit.Name;
        existingUnit.TenantId = unit.TenantId;
        existingUnit.Description = unit.Description ?? existingUnit.Description;
        existingUnit.UpdatedDate = DateTime.UtcNow;
        existingUnit.DueDate = unit.DueDate ?? existingUnit.DueDate;
        existingUnit.ParentUnitId = unit.ParentUnitId ?? existingUnit.ParentUnitId;
        existingUnit.ManagerId = unit.ManagerId ?? existingUnit.ManagerId;
        existingUnit.ManagerName = unit.ManagerName ?? existingUnit.ManagerName;
        existingUnit.Priority = unit.Priority ?? existingUnit.Priority;

        _db.Units.Update(existingUnit);
        await _db.SaveChangesAsync(cancellationToken);

        return existingUnit;
    }

    public async Task<Unit> GetUnitById(Guid id, CancellationToken cancellationToken)
    {
        var unit = await _db.Units.FindAsync(id, cancellationToken);
        if (unit == null)
        {
            throw new KeyNotFoundException("Unit not found");
        }
        return unit;
    }
    
    public async Task<List<Unit>> GetUnitByParentId(Guid parentId, CancellationToken cancellationToken)
    {
        var units = await _db.Units
            .Where(u => u.ParentUnitId == parentId)
            .ToListAsync(cancellationToken);
        return units;
    }
}