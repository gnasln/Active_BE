using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfSvc.Application.Dtos
{
    public class KeyResultHistoryDto
    {
        public Guid Id { get; set; }
        public Guid ObjectTBId { get; set; }
        public Guid keyresultid { get; set; }
        public required string Title { get; set; }
        public string? Description { set; get; } = string.Empty;
        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}