namespace AtlasOps.Features.Architecture.ArchitectureStandardProvisioning;

public sealed record ArchitectureStandardProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);