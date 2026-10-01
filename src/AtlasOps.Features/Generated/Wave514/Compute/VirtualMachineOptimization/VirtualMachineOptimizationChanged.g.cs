namespace AtlasOps.Features.Compute.VirtualMachineOptimization;

public sealed record VirtualMachineOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);