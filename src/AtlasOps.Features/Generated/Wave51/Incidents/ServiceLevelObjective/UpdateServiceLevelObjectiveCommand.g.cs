namespace AtlasOps.Features.Incidents.ServiceLevelObjective;

public sealed record UpdateServiceLevelObjectiveCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);