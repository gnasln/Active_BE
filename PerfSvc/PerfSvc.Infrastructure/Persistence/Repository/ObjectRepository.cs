using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
﻿using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Mappings;
using NetHelper.Common.Models;
using PerfSvc.Domain.Dtos;
using PerfSvc.Domain.Entities;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Infrastructure.Persistence.Repository;

public class ObjectRepository(IPerfDbContext context, IMapper mapper) : IObjectRepository
{
    private readonly IPerfDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<Guid> CreateObject(ObjectTB objectTb, CancellationToken cancellationToken)
    {
        var result = await _context.ObjectTBs.AddAsync(objectTb, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return result.Entity.Id;
    }

    public async Task<ObjectTB> ReadObject(Guid id, CancellationToken cancellationToken)
    {
        var result = await _context.ObjectTBs.FindAsync(id, cancellationToken);
        if (result == null)
        {
            throw new Exception("Object not found");
        }
        return result;
    }

    public async Task<PaginatedList<ObjectDto>> GetAllObject(Guid unitId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var result = from obj in _context.ObjectTBs
            join unit in _context.Units on obj.UnitId equals unit.Id
            where obj.UnitId == unitId
            orderby obj.CreatedDate descending
            select new ObjectDto
            {
                Id = obj.Id,
                Title = obj.Title,
                Description = obj.Description,
                Priority = obj.Priority,
                CreatedDate = obj.CreatedDate,
                UpdatedDate = obj.UpdatedDate,
                DueDate = obj.DueDate,
                UnitId = obj.UnitId,
                UnitName = unit.Name
            };

        return await result.PaginatedListAsync(pageNumber, pageSize);
    }

    public async Task<ObjectTB> UpdateObject(ObjectTB objectTb, CancellationToken cancellationToken)
    {
        var result = await _context.ObjectTBs.FindAsync(objectTb.Id, cancellationToken);
        if (result == null)
        {
            throw new Exception("Object not found");
        }
        result.Title = objectTb.Title ?? result.Title;
        result.Description = objectTb.Description ?? result.Description;
        result.Priority = objectTb.Priority ?? result.Priority;
        result.CreatedDate = objectTb.CreatedDate ?? result.CreatedDate;
        result.UpdatedDate = objectTb.UpdatedDate ?? result.UpdatedDate;
        result.DueDate = objectTb.DueDate ?? result.DueDate;
        _context.ObjectTBs.Update(result);
        await _context.SaveChangesAsync(cancellationToken);
        return result;
    }

    public async Task DeleteObject(Guid id, CancellationToken cancellationToken)
    {
        var result = await _context.ObjectTBs.FindAsync(id, cancellationToken);
        if (result == null)
        {
            throw new Exception("Object not found");
        }
        _context.ObjectTBs.Remove(result);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /* C**** */
    public async Task CreateObjectHistory(ObjectHistory objectHistory, CancellationToken cancellationToken)
    {
        var result = await _context.objectHistories.AddAsync(objectHistory, cancellationToken);
        if(result == null){
            throw new Exception("Object not found");
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
    
}