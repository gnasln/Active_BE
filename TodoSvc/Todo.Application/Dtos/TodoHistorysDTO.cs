using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace TodoSvc.Application.Dtos;

public class TodoHistorysDTO
{
    public Guid Id { get; init; }
    public Guid TodoId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? ModifiedDate { get; init; }

}