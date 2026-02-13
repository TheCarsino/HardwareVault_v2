using System;
using System.Collections.Generic;
using HardwareVault_Services.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HardwareVault_Services.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cpu> Cpus { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceUsbPort> DeviceUsbPorts { get; set; }

    public virtual DbSet<Gpu> Gpus { get; set; }

    public virtual DbSet<ImportJob> ImportJobs { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<PowerSupply> PowerSupplies { get; set; }

    public virtual DbSet<VwDeviceList> VwDeviceLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cpu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cpus__3214EC0738E19AD4");

            entity.HasIndex(e => e.ModelName, "UQ_Cpus_ModelName").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ModelName).HasMaxLength(200);
            entity.Property(e => e.NormalizedModelName).HasMaxLength(200);

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Cpus)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cpus_Manufacturer");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Devices__3214EC07E9B767B7");

            entity.HasIndex(e => new { e.CpuId, e.GpuId, e.PowerSupplyId }, "IX_Devices_CoveringQueries");

            entity.HasIndex(e => e.CreatedAt, "IX_Devices_CreatedAt");

            entity.HasIndex(e => e.IsDeleted, "IX_Devices_IsDeleted").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RamSizeInMb).HasColumnName("RamSizeInMB");
            entity.Property(e => e.StorageSizeInGb).HasColumnName("StorageSizeInGB");
            entity.Property(e => e.StorageType).HasMaxLength(10);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WeightInKg).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Cpu).WithMany(p => p.Devices)
                .HasForeignKey(d => d.CpuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Devices_Cpus");

            entity.HasOne(d => d.Gpu).WithMany(p => p.Devices)
                .HasForeignKey(d => d.GpuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Devices_Gpus");

            entity.HasOne(d => d.PowerSupply).WithMany(p => p.Devices)
                .HasForeignKey(d => d.PowerSupplyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Devices_PowerSupplies");
        });

        modelBuilder.Entity<DeviceUsbPort>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DeviceUs__3214EC076E2B237C");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.UsbPortType).HasMaxLength(20);

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceUsbPorts)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK_DeviceUsbPorts_Devices");
        });

        modelBuilder.Entity<Gpu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gpus__3214EC0702B9711E");

            entity.HasIndex(e => e.ModelName, "UQ_Gpus_ModelName").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ModelName).HasMaxLength(200);
            entity.Property(e => e.NormalizedModelName).HasMaxLength(200);

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Gpus)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gpus_Manufacturer");
        });

        modelBuilder.Entity<ImportJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImportJo__3214EC079F76F955");

            entity.HasIndex(e => e.StartedAt, "IX_ImportJobs_StartedAt");

            entity.HasIndex(e => e.Status, "IX_ImportJobs_Status");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Manufact__3214EC07C999FABB");

            entity.HasIndex(e => e.Name, "UQ_Manufacturers_Name").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ProductType).HasMaxLength(20);
            entity.Property(e => e.Website).HasMaxLength(500);
        });

        modelBuilder.Entity<PowerSupply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PowerSup__3214EC07B5AC48F6");

            entity.HasIndex(e => e.WattageInWatts, "UQ_PowerSupplies_Wattage").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<VwDeviceList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_DeviceList");

            entity.Property(e => e.CpuManufacturer).HasMaxLength(100);
            entity.Property(e => e.CpuModel).HasMaxLength(200);
            entity.Property(e => e.GpuManufacturer).HasMaxLength(100);
            entity.Property(e => e.GpuModel).HasMaxLength(200);
            entity.Property(e => e.RamSizeInMb).HasColumnName("RamSizeInMB");
            entity.Property(e => e.StorageSizeInGb).HasColumnName("StorageSizeInGB");
            entity.Property(e => e.StorageType).HasMaxLength(10);
            entity.Property(e => e.WeightInKg).HasColumnType("decimal(5, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
