using Microsoft.EntityFrameworkCore;
using Demo.Repository.Entities;

namespace Demo.Repository.Datas
{
    public partial class DemoContext : DbContext
    {
        public DemoContext()
        {
        }

        public DemoContext(DbContextOptions<DemoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LogOperator> LogOperators { get; set; } = null!;
        public virtual DbSet<RegisterUser> RegisterUsers { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<SysJwtBlackList> SysJwtBlackLists { get; set; } = null!;
        public virtual DbSet<SysRefTokenList> SysRefTokenLists { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogOperator>(entity =>
            {
                entity.ToTable("LOG_OPERATOR");

                entity.Property(e => e.LogOperatorId)
                    .ValueGeneratedNever()
                    .HasColumnName("LOG_OPERATOR_ID");

                entity.Property(e => e.CreateDtm)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DTM")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Path)
                    .HasMaxLength(200)
                    .HasColumnName("PATH");

                entity.Property(e => e.Request).HasColumnName("REQUEST");

                entity.Property(e => e.Response).HasColumnName("RESPONSE");
            });

            modelBuilder.Entity<RegisterUser>(entity =>
            {
                entity.ToTable("REGISTER_USER");

                entity.Property(e => e.RegisterUserId)
                    .ValueGeneratedNever()
                    .HasColumnName("REGISTER_USER_ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Password)
                    .HasMaxLength(16)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnName("PHONE_NUMBER");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("USER_NAME");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLE");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("ROLE_ID");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("ROLE_NAME");
            });

            modelBuilder.Entity<SysJwtBlackList>(entity =>
            {
                entity.ToTable("SYS_JWT_BLACK_LIST");

                entity.Property(e => e.SysJwtBlackListId)
                    .ValueGeneratedNever()
                    .HasColumnName("SYS_JWT_BLACK_LIST_ID");

                entity.Property(e => e.CreateDtm)
                    .HasColumnType("date")
                    .HasColumnName("CREATE_DTM")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.JwtToken).HasColumnName("JWT_TOKEN");
            });

            modelBuilder.Entity<SysRefTokenList>(entity =>
            {
                entity.ToTable("SYS_REF_TOKEN_LIST");

                entity.Property(e => e.SysRefTokenListId)
                    .ValueGeneratedNever()
                    .HasColumnName("SYS_REF_TOKEN_LIST_ID");

                entity.Property(e => e.CreateDtm)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DTM")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("EFFECTIVE_DATE");

                entity.Property(e => e.RefToken).HasColumnName("REF_TOKEN");

                entity.Property(e => e.UserId).HasColumnName("USER_ID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("USER_ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Password)
                    .HasMaxLength(16)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnName("PHONE_NUMBER");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("USER_NAME");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("USER_ROLE");

                entity.Property(e => e.UserRoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("USER_ROLE_ID");

                entity.Property(e => e.RoleId).HasColumnName("ROLE_ID");

                entity.Property(e => e.UserId).HasColumnName("USER_ID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USER_ROLE_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USER_ROLE_UserId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
