namespace AtlasOps.Features.Compute.VirtualMachineProvisioning;

public sealed record VirtualMachineProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);