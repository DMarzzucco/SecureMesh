using System;
using Auth.Module.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Context.Config;

public class AuthModelConfig : IEntityTypeConfiguration<AuthModel>
{
    public void Configure(EntityTypeBuilder<AuthModel> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).UseIdentityColumn().ValueGeneratedOnAdd();

        builder.Property(r => r.UserId).IsRequired(true);

        builder.Property(r => r.IsDeleted).IsRequired();
        builder.Property(r => r.DeletedAt).IsRequired(false);
        builder.Property(r => r.ScheduledDeletionJobId).IsRequired(false);

        builder.Property(r => r.TwoFACode).IsRequired(false);
        builder.Property(r => r.TwoFACodeExpiration).IsRequired(false);
        builder.Property(r => r.LockedAt).IsRequired(false);

        builder.Property(r => r.RefreshToken).IsRequired(false);

        builder.ToTable("Auth");
    }
}
