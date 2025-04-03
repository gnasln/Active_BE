using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoSvc.Domain.Entities;
public class TodoHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid TodoId {get; set;}
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ModifiedDate { get; set; } = DateTime.Now;
}