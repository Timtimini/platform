using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OTITO_Services.Model
{
    public partial class OtitoDBContext : DbContext
    {
        public OtitoDBContext()
        {
        }

        public OtitoDBContext(DbContextOptions<OtitoDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Claim> Claim { get; set; }
        public virtual DbSet<Source> Source { get; set; }
        public virtual DbSet<Topic> Topic { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Vote> Vote { get; set; }
        public virtual DbSet<Activity> Activity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

               // optionsBuilder.UseMySQL(Configuration.GetConnectionString("otito")");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.ToTable("claim", "otito_v3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClaimDescription)
                    .IsRequired()
                    .HasColumnName("Claim_Description")
                    .HasMaxLength(240)
                    .IsUnicode(false);

                entity.Property(e => e.ClaimId).HasColumnType("int(11)");

                entity.Property(e => e.ClaimTitle)
                    .IsRequired()
                    .HasColumnName("Claim_Title")
                    .HasMaxLength(140)
                    .IsUnicode(false);

                entity.Property(e => e.Slug)
                  
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Guid)
          
                    .HasColumnName("Guid")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.ClaimGuid)

                    .HasColumnName("ClaimGuid")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasColumnType("tinyint(1)");

                entity.Property(e => e.TopicId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");
                entity.Property(e => e.Sources).HasColumnType("int(11)");
                entity.Property(e => e.Vote).HasColumnType("int(11)");
                entity.Property(e => e.UserId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.ToTable("Source", "otito_v3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClaimId).HasColumnType("int(11)");

                entity.Property(e => e.DownVote).HasColumnType("int(11)");

                entity.Property(e => e.IsDeleted).HasColumnType("tinyint(1)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Guid)
                 
                    .HasColumnName("Guid")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Slug)
                   
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.UpVote).HasColumnType("int(11)");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Vote).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("topic", "otito_v3");

                entity.Property(e => e.Id).HasColumnType("int(11)");
               
                entity.Property(e => e.GradientName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasColumnType("tinyint(1)");

                entity.Property(e => e.TopicName)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Slug)
                  
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Guid)

                   
                   .HasMaxLength(12)
                   .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnType("int(11)");
                entity.Property(e => e.IsSticked).HasColumnType("tinyint(1)");

            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("activity", "otito_v3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.activity_type)
                    .HasMaxLength(45).IsRequired()
                    .IsUnicode(false);



                entity.Property(e => e.activity_title)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnType("int(11)");
                entity.Property(e => e.TopicId).HasColumnType("int(11)");
                entity.Property(e => e.ClaimId).HasColumnType("int(11)");

            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "otito_v3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.City)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasColumnName("First_Name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasColumnName("Last_Name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NegativeKarma).HasColumnType("int(11)");

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.PositiveKarma).HasColumnType("int(11)");
                entity.Property(e => e.TotalVote).HasColumnType("int(11)");

                entity.Property(e => e.Role)
                    .HasColumnName("Role")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsSocial).HasColumnType("tinyint(1)");
            });

            modelBuilder.Entity<Vote>(entity =>
            {
                entity.ToTable("Vote", "otito_v3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.SourceId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.Vote1)
                    .HasColumnName("Vote")
                    .HasColumnType("int(11)");
            });
        }
    }
}
