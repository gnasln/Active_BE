using NetHelper.Common.Models;
using PerfSvc.Domain.Entities;

namespace PerfSvc.Infrastructure.Interface.Repository;

public interface IKeyResultRepository
{
    public Task<Guid> CreateKeyResult(KeyResult keyResult, CancellationToken cancellationToken);
    public Task CreateKeyResultHistory(KeyresultHistory keyresultHistory, CancellationToken cancellationToken);
    public Task<KeyResult> ReadKeyResult(Guid id, CancellationToken cancellationToken);
    public Task<PaginatedList<KeyResult>> GetAllKeyResult(Guid objectId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<KeyResult> UpdateKeyResult(KeyResult keyResult, CancellationToken cancellationToken);
    public Task DeleteKeyResult(Guid id, CancellationToken cancellationToken);
    public Task<List<KeyResult>> SearchKeyResult(string keyword, CancellationToken cancellationToken);
}