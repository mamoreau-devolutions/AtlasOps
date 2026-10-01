namespace AtlasOps.Features.Architecture.ArchitectureEvidenceMonitoring;

public sealed record UpdateArchitectureEvidenceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);