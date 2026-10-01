namespace AtlasOps.Features.Incidents.IncidentCase;

public sealed record UpdateIncidentCaseCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);