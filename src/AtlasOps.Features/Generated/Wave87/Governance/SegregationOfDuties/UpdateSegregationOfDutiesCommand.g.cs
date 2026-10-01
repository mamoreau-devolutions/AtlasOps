namespace AtlasOps.Features.Governance.SegregationOfDuties;

public sealed record UpdateSegregationOfDutiesCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);