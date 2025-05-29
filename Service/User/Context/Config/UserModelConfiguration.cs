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
        
        builder.Property(r => r.FullName).IsRequired().HasMaxLength(50);
        
        builder.Property(r => r.Username).IsUnicode().IsRequired().HasMaxLength(50);
        builder.Property(r => r.Email).IsUnicode().IsRequired().HasMaxLength(50);

        builder.Property(r=> r.EmailVerified).IsRequired();

        builder.Property(r => r.Password).IsRequired();

        builder.Property(r => r.Roles)
            .HasConversion(EnumConversionHelper.GetEnumConverter<ROLES>())
            .HasMaxLength(20).IsUnicode(false).IsRequired();

        builder.Property(r => r.IsDeleted).IsRequired();
        builder.Property(r => r.DeletedAt).IsRequired(false);
        builder.Property(r => r.ScheduledDeletionJobId).IsRequired(false);

        builder.Property(r => r.RefreshToken).IsRequired(false);
        
        builder.ToTable("User");
    }
}