// ============================================================
// DOMAIN LAYER — PARTIAL BUSINESS CLASSES
// ============================================================
// DATABASE-FIRST PATTERN:
//
//   Infrastructure/Data/Entities/Device.cs         ← SCAFFOLDED (data props)
//   Infrastructure/Data/Entities/Device.Partial.cs ← THIS FILE (business logic)
//
// Both are declared as: public partial class Device { }
// The C# compiler merges them into one class at build time.
//
// RULE: Never edit the scaffolded file — only this one.
//       When the schema changes -> re-scaffold -> re-apply partial additions.
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using HardwareVault_Services.Domain.Enums;

namespace HardwareVault_Services.Infrastructure.Data.Entities
{
    public partial class Device
    {
        // -- Factory Method 
        // The ONLY valid way to create a new Device from outside this class.
        // Validates all business rules before the object can exist.
        public static Device Create(
            int     ramSizeInMb,
            int     storageSizeInGb,
            string  storageType,      // "SSD" or "HDD" — matches DB string column
            int     cpuId,
            int     gpuId,
            int     powerSupplyId,
            decimal weightInKg)
        {
            ValidateRamSize(ramSizeInMb);
            ValidateStorageSize(storageSizeInGb);
            ValidateStorageType(storageType);
            ValidatePositiveId(cpuId,         nameof(cpuId));
            ValidatePositiveId(gpuId,         nameof(gpuId));
            ValidatePositiveId(powerSupplyId, nameof(powerSupplyId));
            ValidateWeight(weightInKg);

            return new Device
            {
                Id              = Guid.NewGuid(),
                RamSizeInMb     = ramSizeInMb,
                StorageSizeInGb = storageSizeInGb,
                StorageType     = storageType.ToUpperInvariant(),
                CpuId           = cpuId,
                GpuId           = gpuId,
                PowerSupplyId   = powerSupplyId,
                WeightInKg      = weightInKg,
                CreatedAt       = DateTime.UtcNow,
                UpdatedAt       = DateTime.UtcNow,
                IsDeleted       = false,
                DeviceUsbPorts  = new List<DeviceUsbPort>()
            };
        }

        // -- Business Methods 
        // You cannot do device.IsDeleted = true from outside.
        // You call device.SoftDelete(). The method owns what that means.

        public void SoftDelete()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Device is already deleted.");
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted)
                throw new InvalidOperationException("Device is not currently deleted.");
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRam(int newRamSizeInMb)
        {
            ValidateRamSize(newRamSizeInMb);
            RamSizeInMb = newRamSizeInMb;
            UpdatedAt   = DateTime.UtcNow;
        }

        public void UpdateStorage(int newStorageSizeInGb, string newStorageType)
        {
            ValidateStorageSize(newStorageSizeInGb);
            ValidateStorageType(newStorageType);
            StorageSizeInGb = newStorageSizeInGb;
            StorageType     = newStorageType.ToUpperInvariant();
            UpdatedAt       = DateTime.UtcNow;
        }

        public void UpdateWeight(decimal newWeightInKg)
        {
            ValidateWeight(newWeightInKg);
            WeightInKg = newWeightInKg;
            UpdatedAt  = DateTime.UtcNow;
        }

        public void UpdateCpu(int newCpuId)
        {
            ValidatePositiveId(newCpuId, nameof(newCpuId));
            CpuId     = newCpuId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateGpu(int newGpuId)
        {
            ValidatePositiveId(newGpuId, nameof(newGpuId));
            GpuId     = newGpuId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePowerSupply(int newPowerSupplyId)
        {
            ValidatePositiveId(newPowerSupplyId, nameof(newPowerSupplyId));
            PowerSupplyId = newPowerSupplyId;
            UpdatedAt     = DateTime.UtcNow;
        }

        public void AddUsbPort(string portType, int portCount)
        {
            if (string.IsNullOrWhiteSpace(portType))
                throw new ArgumentException("Port type cannot be empty.", nameof(portType));
            if (portCount <= 0)
                throw new ArgumentException("Port count must be positive.", nameof(portCount));

            DeviceUsbPorts.Add(new DeviceUsbPort
            {
                Id          = Guid.NewGuid(),
                DeviceId    = Id,
                UsbPortType = portType,
                PortCount   = portCount
            });
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearUsbPorts()
        {
            DeviceUsbPorts.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        // -- Computed Read-Only Properties 
        // These do NOT map to DB columns — they are derived from stored values.
        public int RamSizeInGb => RamSizeInMb / 1024;
        public int TotalUsbPorts => DeviceUsbPorts.Sum(p => p.PortCount);

        // -- Private Validators 
        private static void ValidateRamSize(int mb)
        {
            if (mb < 512)
                throw new ArgumentException("RAM must be at least 512 MB.", nameof(mb));
            if (mb > 1_048_576)
                throw new ArgumentException("RAM cannot exceed 1 TB.", nameof(mb));
        }

        private static void ValidateStorageSize(int gb)
        {
            if (gb < 1)
                throw new ArgumentException("Storage must be at least 1 GB.", nameof(gb));
            if (gb > 100_000)
                throw new ArgumentException("Storage cannot exceed 100 TB.", nameof(gb));
        }

        private static void ValidateStorageType(string type)
        {
            if (type.ToUpperInvariant() is not "SSD" and not "HDD")
                throw new ArgumentException("Storage type must be 'SSD' or 'HDD'.", nameof(type));
        }

        private static void ValidateWeight(decimal kg)
        {
            if (kg < 0.1m)
                throw new ArgumentException("Weight must be at least 0.1 kg.", nameof(kg));
            if (kg > 500m)
                throw new ArgumentException("Weight cannot exceed 500 kg.", nameof(kg));
        }

        private static void ValidatePositiveId(int id, string paramName)
        {
            if (id <= 0)
                throw new ArgumentException($"{paramName} must be a positive integer.", paramName);
        }
    }
}
