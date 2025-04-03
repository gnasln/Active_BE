using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bff.Domain.Entities
{
    public class BackupDataTenantMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { set; get; }
        public Guid UserId { set; get; }
        public  string UserName { set; get; }
        public string UserFullName { set; get; }
        public Guid TenantId { set; get; }
    }
}