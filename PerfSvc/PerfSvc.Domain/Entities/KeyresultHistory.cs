using PrefSvc.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PerfSvc.Domain.Entities
{
    public class KeyresultHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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