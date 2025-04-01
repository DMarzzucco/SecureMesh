using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Module.Enums;
using User.Module.Model;
using User.Utils.Helper;

namespace User.Context.Config;

public class UserModelConfiguration:IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).UseIdentityColumn().ValueGeneratedOnAdd();
        
        builder.Property(r => r.complete_name).IsRequired().HasMaxLength(50);
        
        builder.Property(r => r.username).IsUnicode().IsRequired().HasMaxLength(50);
        builder.Property(r => r.email).IsUnicode().IsRequired().HasMaxLength(50);
        builder.Property(r => r.password);
        
        builder.Property(r => r.roles)
            .HasConversion(EnumConversionHelper.GetEnumConverter<ROLES>()).HasMaxLength(20).IsUnicode(false).IsRequired();
        
        builder.Property(r => r.RefreshToken).IsRequired(false);
        
        builder.ToTable("User");
    }
}