using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _66076431_natalee.Models.db;

public partial class Csi402dbContext : DbContext
{
    public Csi402dbContext()
    {
    }

    public Csi402dbContext(DbContextOptions<Csi402dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<LabStudent> LabStudents { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE488A930CAE8");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId).HasMaxLength(10);
            entity.Property(e => e.AdminEmail).HasMaxLength(50);
            entity.Property(e => e.AdminName).HasMaxLength(50);
            entity.Property(e => e.AdminPassword).HasMaxLength(30);
            entity.Property(e => e.AdminPhoneNumber).HasMaxLength(10);
            entity.Property(e => e.AdminRole).HasMaxLength(30);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CusId).HasName("PK__Customer__2F187110BFB78318");

            entity.ToTable("Customer");

            entity.Property(e => e.CusId).HasMaxLength(10);
            entity.Property(e => e.CusAddress).HasMaxLength(200);
            entity.Property(e => e.CusName).HasMaxLength(50);
            entity.Property(e => e.CusPassword).HasMaxLength(30);
            entity.Property(e => e.CusPhoneNumber).HasMaxLength(10);
            entity.Property(e => e.CusrEmail).HasMaxLength(50);
        });

        modelBuilder.Entity<LabStudent>(entity =>
        {
            entity.HasKey(e => e.Stdld).HasName("PK__LabStude__55DB59A3435BB80D");

            entity.ToTable("LabStudent");

            entity.Property(e => e.Stdld).HasMaxLength(10);
            entity.Property(e => e.StdldLastname).HasMaxLength(100);
            entity.Property(e => e.StdldName).HasMaxLength(50);
            entity.Property(e => e.StdldPassword).HasMaxLength(30);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF1A023E1A");

            entity.Property(e => e.OrderId).ValueGeneratedNever();
            entity.Property(e => e.CusId).HasMaxLength(10);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderNo).HasMaxLength(20);
            entity.Property(e => e.OrderStatus).HasMaxLength(30);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
            entity.Property(e => e.ShipAddress).HasMaxLength(200);
            entity.Property(e => e.ShippingMethod).HasMaxLength(50);

            entity.HasOne(d => d.Cus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CusId)
                .HasConstraintName("FK__Orders__CusId__71D1E811");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36CB479DF12");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).ValueGeneratedNever();
            entity.Property(e => e.ProductId).HasMaxLength(10);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__74AE54BC");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__OrderDeta__Produ__75A278F5");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6CD87D07491");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasMaxLength(10);
            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.DescriptionPd)
                .HasMaxLength(500)
                .HasColumnName("DescriptionPD");
            entity.Property(e => e.ImageUrl).HasMaxLength(300);
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.Warranty).HasMaxLength(30);
        });
OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
