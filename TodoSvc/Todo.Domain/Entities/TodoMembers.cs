using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoSvc.Domain.Entities;

namespace Todo.Domain.Entities
{
    public class TodoMembers
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(TodoItem))]
        public Guid TodoId { get; set; }
        public Guid MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberFullName { get; set; }
    }
}