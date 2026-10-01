namespace AtlasOps.Features.Architecture.ArchitectureDependencyProvisioning;

public sealed record ArchitectureDependencyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);