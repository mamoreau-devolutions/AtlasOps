namespace AtlasOps.Features.Compute.VirtualMachineRecovery;

public sealed record VirtualMachineRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);