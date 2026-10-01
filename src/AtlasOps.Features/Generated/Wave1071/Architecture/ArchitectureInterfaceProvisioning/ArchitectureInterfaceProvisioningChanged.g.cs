namespace AtlasOps.Features.Architecture.ArchitectureInterfaceProvisioning;

public sealed record ArchitectureInterfaceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);