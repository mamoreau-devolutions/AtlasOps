namespace AtlasOps.Features.Compute.ComputeReservationGovernance;

public sealed record UpdateComputeReservationGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);