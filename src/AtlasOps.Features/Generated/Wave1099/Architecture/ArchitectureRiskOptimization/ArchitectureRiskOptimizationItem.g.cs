namespace AtlasOps.Features.Architecture.ArchitectureRiskOptimization;

using AtlasOps.Features;

public sealed class ArchitectureRiskOptimizationItem : IAtlasOpsCapabilityEntity
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Name { get; set; } = "Architecture Risk Optimization";

    public string Owner { get; set; } = "Operations";

    public string State { get; set; } = "Draft";

    public int Priority { get; set; } = 9;

    public bool IsEnabled { get; set; } = true;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public int Revision { get; private set; }

    public void MarkUpdated(DateTimeOffset timestamp)
    {
        this.UpdatedAt = timestamp;
        this.Revision++;
    }
}