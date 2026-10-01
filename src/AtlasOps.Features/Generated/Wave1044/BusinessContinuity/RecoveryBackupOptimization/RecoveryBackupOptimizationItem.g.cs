namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupOptimization;

using AtlasOps.Features;

public sealed class RecoveryBackupOptimizationItem : IAtlasOpsCapabilityEntity
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Name { get; set; } = "Recovery Backup Optimization";

    public string Owner { get; set; } = "Operations";

    public string State { get; set; } = "Draft";

    public int Priority { get; set; } = 4;

    public bool IsEnabled { get; set; } = true;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public int Revision { get; private set; }

    public void MarkUpdated(DateTimeOffset timestamp)
    {
        this.UpdatedAt = timestamp;
        this.Revision++;
    }
}