namespace AtlasOps.Features.Incidents.IncidentTimeline;

public sealed record UpdateIncidentTimelineCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);