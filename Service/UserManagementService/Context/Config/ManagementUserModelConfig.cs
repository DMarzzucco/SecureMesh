using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementService.Modules.Models;

namespace UserManagementService.Context.Config;

public class ManagementUserModelConfig : IEntityTypeConfiguration<ManagementUserModel>
{
    public void Configure(EntityTypeBuilder<ManagementUserModel> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).UseIdentityColumn().ValueGeneratedOnAdd();

        builder.Property(r => r.UserId).IsRequired(true);

        builder.Property(r => r.IsDeleted).IsRequired();
        builder.Property(r => r.DeletedAt).IsRequired(false);
        builder.Property(r => r.ScheduledDeletionJobId).IsRequired(false);

        builder.Property(r => r.IsDisabled).IsRequired();

        builder.Property(r => r.LockedAt).IsRequired(false);

        builder.ToTable("management_user");
    }
}
