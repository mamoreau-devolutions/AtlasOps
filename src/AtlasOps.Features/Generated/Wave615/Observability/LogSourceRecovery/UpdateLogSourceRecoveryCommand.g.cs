namespace AtlasOps.Features.Observability.LogSourceRecovery;

public sealed record UpdateLogSourceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);