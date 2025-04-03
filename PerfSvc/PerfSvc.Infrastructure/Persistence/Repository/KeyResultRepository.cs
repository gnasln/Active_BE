using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Mappings;
using NetHelper.Common.Models;
using PerfSvc.Domain.Entities;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Infrastructure.Persistence.Repository;

public class KeyResultRepository(IPerfDbContext context) : IKeyResultRepository
{
    private readonly IPerfDbContext _context = context;
    public async Task<Guid> CreateKeyResult(KeyResult keyResult, CancellationToken cancellationToken)
    {
        await _context.KeyResults.AddAsync(keyResult, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return keyResult.Id;
    }

    public async Task<KeyResult> ReadKeyResult(Guid id, CancellationToken cancellationToken)
    {
        var keyResult = await _context.KeyResults.FindAsync(id, cancellationToken);
        if(keyResult == null)
        {
            throw new Exception("KeyResult not found");
        }
        return keyResult;
    }

    public async Task<PaginatedList<KeyResult>> GetAllKeyResult(Guid objectId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var keyResults = _context.KeyResults.AsQueryable().Where(x => x.ObjectTBId == objectId);
        return await keyResults.OrderByDescending(x => x.CreatedDate).PaginatedListAsync(pageNumber, pageSize);
    }

    public async Task<KeyResult> UpdateKeyResult(KeyResult keyResult, CancellationToken cancellationToken)
    {
        var keyResultUpdate = await _context.KeyResults.FindAsync(keyResult.Id, cancellationToken);
        if(keyResultUpdate == null)
        {
            throw new Exception("KeyResult not found");
        }
        keyResultUpdate.Title = keyResult.Title ?? keyResultUpdate.Title;
        keyResultUpdate.Description = keyResult.Description ?? keyResultUpdate.Description;
        keyResultUpdate.DueDate = keyResult.DueDate ?? keyResultUpdate.DueDate;
        keyResultUpdate.Priority = keyResult.Priority ?? keyResultUpdate.Priority;
        keyResultUpdate.ObjectTBId = keyResult.ObjectTBId;
        _context.KeyResults.Update(keyResultUpdate);
        await _context.SaveChangesAsync(cancellationToken);
        return keyResultUpdate;
    }

    public async Task DeleteKeyResult(Guid id, CancellationToken cancellationToken)
    {
        var keyResult = await _context.KeyResults.FindAsync(id, cancellationToken);
        if(keyResult == null)
        {
            throw new Exception("KeyResult not found");
        }
        _context.KeyResults.Remove(keyResult);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<KeyResult>> SearchKeyResult(string keyword, CancellationToken cancellationToken)
    {
        var keyResults = _context.KeyResults.Where(x => x.Title.Contains(keyword)).ToListAsync(cancellationToken);
        return keyResults;
    }

    public Task CreateKeyResultHistory(KeyresultHistory keyresultHistory, CancellationToken cancellationToken)
    {
        _context.KeyresultHistories.Add(keyresultHistory);
        return _context.SaveChangesAsync(cancellationToken);
    }
}