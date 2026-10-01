namespace AtlasOps.Features.Architecture.ArchitectureExceptionProvisioning;

public sealed record ArchitectureExceptionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);