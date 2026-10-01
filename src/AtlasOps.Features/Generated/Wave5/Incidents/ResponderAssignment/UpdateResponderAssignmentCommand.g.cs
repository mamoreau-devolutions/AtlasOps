namespace AtlasOps.Features.Incidents.ResponderAssignment;

public sealed record UpdateResponderAssignmentCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);