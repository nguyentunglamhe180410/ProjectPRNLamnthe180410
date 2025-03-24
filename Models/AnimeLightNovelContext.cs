using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRNLamnthe180410.Models;

public partial class AnimeLightNovelContext : DbContext
{
    public AnimeLightNovelContext()
    {
    }

    public AnimeLightNovelContext(DbContextOptions<AnimeLightNovelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bought> Boughts { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<LightNovel> LightNovels { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(ConnectionString);
        }

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bought>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bought__3214EC27040BF5BC");

            entity.ToTable("Bought");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TitleId).HasColumnName("TitleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Title).WithMany(p => p.Boughts)
                .HasForeignKey(d => d.TitleId)
                .HasConstraintName("FK__Bought__TitleID__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.Boughts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bought__UserID__52593CB8");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC273768AAA8");

            entity.ToTable("Comment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LightNovelId).HasColumnName("LightNovelID");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.LightNovel).WithMany(p => p.Comments)
                .HasForeignKey(d => d.LightNovelId)
                .HasConstraintName("FK__Comment__LightNo__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comment__UserID__571DF1D5");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genre__3214EC27B8D7B58A");

            entity.ToTable("Genre");

            entity.HasIndex(e => e.Name, "UQ__Genre__737584F6E4885C66").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History__3214EC272064D43F");

            entity.ToTable("History");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PurchasedDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Histories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__History__UserID__5CD6CB2B");
        });

        modelBuilder.Entity<LightNovel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LightNov__3214EC27B0444D80");

            entity.ToTable("LightNovel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cost).HasDefaultValue(200);
            entity.Property(e => e.GenreId)
                .HasDefaultValue(1)
                .HasColumnName("GenreID");
            entity.Property(e => e.ImgUrl).HasColumnName("Img_Url");
            entity.Property(e => e.Read).HasDefaultValue(0);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Genre).WithMany(p => p.LightNovels)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK__LightNove__Genre__6C190EBB");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC27F2B18EE2");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105342EF349E2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Coins).HasDefaultValue(0);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
