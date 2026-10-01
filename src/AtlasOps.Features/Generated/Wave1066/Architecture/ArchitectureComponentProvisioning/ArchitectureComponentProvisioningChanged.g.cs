namespace AtlasOps.Features.Architecture.ArchitectureComponentProvisioning;

public sealed record ArchitectureComponentProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);