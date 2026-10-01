namespace AtlasOps.Features.Compute.VirtualMachineGovernance;

public sealed record VirtualMachineGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);