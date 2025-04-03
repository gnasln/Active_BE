using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bff.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;

namespace Bff.Application.Backsup.Command
{
    public record ResetDataCommand : IRequest<ResultCustom<string>>
    {
        public Guid TenantId { get; set; }
    }

    public class ResetDataUnit : IRequestHandler<ResetDataCommand, ResultCustom<string>>
    {
        private readonly IApplicationDbContext _db;

        public ResetDataUnit(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ResultCustom<string>> Handle(ResetDataCommand rq, CancellationToken cancellationToken)
        {
            try
            {
                // Kiểm tra Tenant có tồn tại không
                var tenant = await _db.Tenants
                                        .FirstOrDefaultAsync(x => x.Id == rq.TenantId, cancellationToken);
                if (tenant == null)
                {
                    return new ResultCustom<string>
                    {
                        Status = StatusCode.NOTFOUND,
                        Message = new[] { "Tenant Id doesn't exist" }
                    };
                }

                // Kiểm tra TenantMember có tồn tại không
                var tenantMember = await _db.TenantMembers
                                            .Where(x => x.TenantId == rq.TenantId)
                                            .ToListAsync(cancellationToken);

                if (tenantMember == null || tenantMember.Count == 0)
                {
                    return new ResultCustom<string>
                    {
                        Status = StatusCode.NOTFOUND,
                        Message = new[] { "TenantMember Id doesn't exist" }
                    };
                }

                //kiểm tra unit có tồn tại không !

                // Lưu thông tin Tenant vào bảng BackupDataTenant
                var backupTenant = new Bff.Domain.Entities.BackupDataTenant
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Owner = tenant.Owner,
                    OwnerName = tenant.OwnerName,
                    Description = tenant.Description,
                    CreatedDate = tenant.CreatedDate,
                    IsWorkSpacePersonal = tenant.IsWorkSpacePersonal
                };

                await _db.BackupDataTenants.AddAsync(backupTenant, cancellationToken);

                // Lưu thông tin TenantMember vào bảng BackupDataTenantMember
                foreach (var member in tenantMember)
                {
                    var backupTenantMember = new Bff.Domain.Entities.BackupDataTenantMember
                    {
                        Id = member.Id,
                        UserId = member.UserId,
                        UserName = member.UserName,
                        UserFullName = member.UserFullName,
                        TenantId = tenant.Id
                    };

                    await _db.BackupDataTenantMembers.AddAsync(backupTenantMember, cancellationToken);
                }

                // Xóa dữ liệu trong bảng Tenants và TenantMembers
                _db.Tenants.Remove(tenant);
                _db.TenantMembers.RemoveRange(tenantMember);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _db.SaveChangesAsync(cancellationToken);

                return new ResultCustom<string>
                {
                    Status = StatusCode.OK,
                    Message = new[] { "Tenant data and members successfully backed up and deleted." }
                };
            }
            catch (Exception ex)
            {
                return new ResultCustom<string>
                {
                    Status = StatusCode.INTERNALSERVERERROR,
                    Message = new[] { $"Error: {ex.Message}" }
                };
            }
        }
    }
}
