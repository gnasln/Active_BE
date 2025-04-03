using NetHelper.Common.Models;
using PerfSvc.Domain.Dtos;
using PerfSvc.Domain.Entities;

namespace PerfSvc.Infrastructure.Interface.Repository;

public interface IObjectRepository
{
    public Task<Guid> CreateObject(ObjectTB objectTb, CancellationToken cancellationToken);
    public Task CreateObjectHistory(ObjectHistory objectHistory, CancellationToken cancellationToken);
    public Task<ObjectTB> ReadObject(Guid id, CancellationToken cancellationToken);
    public Task<PaginatedList<ObjectDto>> GetAllObject(Guid unitId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<ObjectTB> UpdateObject(ObjectTB objectTb, CancellationToken cancellationToken);
    public Task DeleteObject(Guid id, CancellationToken cancellationToken);
}