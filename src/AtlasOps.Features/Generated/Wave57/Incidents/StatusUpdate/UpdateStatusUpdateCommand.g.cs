namespace AtlasOps.Features.Incidents.StatusUpdate;

public sealed record UpdateStatusUpdateCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);