namespace AtlasOps.Features.Incidents.IncidentActionItem;

public sealed record UpdateIncidentActionItemCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);