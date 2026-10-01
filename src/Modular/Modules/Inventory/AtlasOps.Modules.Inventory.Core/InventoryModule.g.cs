namespace AtlasOps.Modules.Inventory.Core;

using System.Collections.Generic;

public sealed record InventoryCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class InventoryModule
{
    public const string Id = "Inventory";
    public const string DisplayName = "Assets and inventory";
    public static IReadOnlyList<InventoryCapabilityDescriptor> Capabilities { get; } = new InventoryCapabilityDescriptor[]
    {
        new("Inventory.AssetInventory", "Asset inventory", "Asset", "Inventory", "Coordinates assets and inventory for Asset inventory."),
        new("Inventory.AssetClassification", "Asset classification", "Asset", "Classification", "Coordinates assets and inventory for Asset classification."),
        new("Inventory.AssetDrift", "Asset drift", "Asset", "Drift", "Coordinates assets and inventory for Asset drift."),
        new("Inventory.AssetOwnership", "Asset ownership", "Asset", "Ownership", "Coordinates assets and inventory for Asset ownership."),
        new("Inventory.AssetLifecycle", "Asset lifecycle", "Asset", "Lifecycle", "Coordinates assets and inventory for Asset lifecycle."),
        new("Inventory.AssetReporting", "Asset reporting", "Asset", "Reporting", "Coordinates assets and inventory for Asset reporting."),
        new("Inventory.DeviceInventory", "Device inventory", "Device", "Inventory", "Coordinates assets and inventory for Device inventory."),
        new("Inventory.DeviceClassification", "Device classification", "Device", "Classification", "Coordinates assets and inventory for Device classification."),
        new("Inventory.DeviceDrift", "Device drift", "Device", "Drift", "Coordinates assets and inventory for Device drift."),
        new("Inventory.DeviceOwnership", "Device ownership", "Device", "Ownership", "Coordinates assets and inventory for Device ownership."),
        new("Inventory.DeviceLifecycle", "Device lifecycle", "Device", "Lifecycle", "Coordinates assets and inventory for Device lifecycle."),
        new("Inventory.DeviceReporting", "Device reporting", "Device", "Reporting", "Coordinates assets and inventory for Device reporting."),
        new("Inventory.SoftwareInventory", "Software inventory", "Software", "Inventory", "Coordinates assets and inventory for Software inventory."),
        new("Inventory.SoftwareClassification", "Software classification", "Software", "Classification", "Coordinates assets and inventory for Software classification."),
        new("Inventory.SoftwareDrift", "Software drift", "Software", "Drift", "Coordinates assets and inventory for Software drift."),
        new("Inventory.SoftwareOwnership", "Software ownership", "Software", "Ownership", "Coordinates assets and inventory for Software ownership."),
        new("Inventory.SoftwareLifecycle", "Software lifecycle", "Software", "Lifecycle", "Coordinates assets and inventory for Software lifecycle."),
        new("Inventory.SoftwareReporting", "Software reporting", "Software", "Reporting", "Coordinates assets and inventory for Software reporting."),
        new("Inventory.DependencyInventory", "Dependency inventory", "Dependency", "Inventory", "Coordinates assets and inventory for Dependency inventory."),
        new("Inventory.DependencyClassification", "Dependency classification", "Dependency", "Classification", "Coordinates assets and inventory for Dependency classification."),
        new("Inventory.DependencyDrift", "Dependency drift", "Dependency", "Drift", "Coordinates assets and inventory for Dependency drift."),
        new("Inventory.DependencyOwnership", "Dependency ownership", "Dependency", "Ownership", "Coordinates assets and inventory for Dependency ownership."),
        new("Inventory.DependencyLifecycle", "Dependency lifecycle", "Dependency", "Lifecycle", "Coordinates assets and inventory for Dependency lifecycle."),
        new("Inventory.DependencyReporting", "Dependency reporting", "Dependency", "Reporting", "Coordinates assets and inventory for Dependency reporting."),
        new("Inventory.DiscoveryInventory", "Discovery inventory", "Discovery", "Inventory", "Coordinates assets and inventory for Discovery inventory."),
        new("Inventory.DiscoveryClassification", "Discovery classification", "Discovery", "Classification", "Coordinates assets and inventory for Discovery classification."),
        new("Inventory.DiscoveryDrift", "Discovery drift", "Discovery", "Drift", "Coordinates assets and inventory for Discovery drift."),
        new("Inventory.DiscoveryOwnership", "Discovery ownership", "Discovery", "Ownership", "Coordinates assets and inventory for Discovery ownership."),
        new("Inventory.DiscoveryLifecycle", "Discovery lifecycle", "Discovery", "Lifecycle", "Coordinates assets and inventory for Discovery lifecycle."),
        new("Inventory.DiscoveryReporting", "Discovery reporting", "Discovery", "Reporting", "Coordinates assets and inventory for Discovery reporting."),
    };
}