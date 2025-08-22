using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionsManagement.Module.Model;

namespace SessionsManagement.Context.Config;

public class SecurirtyModelConfig : IEntityTypeConfiguration<SessionModel>
{
    public void Configure(EntityTypeBuilder<SessionModel> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).UseIdentityColumn().ValueGeneratedOnAdd();

        builder.Property(r => r.UserId).IsRequired(true);

        builder.Property(r => r.Ip).IsRequired(true);
        builder.Property(r => r.UserAgent).IsRequired(true);
        builder.Property(r => r.Location).IsRequired(true);
        
        builder.ToTable("Sessions");
    }
}
