namespace AtlasOps.Features.Incidents.OnCallSchedule;

public sealed record UpdateOnCallScheduleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);