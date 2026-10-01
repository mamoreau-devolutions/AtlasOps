namespace AtlasOps.Features.Architecture.ArchitectureRiskProvisioning;

public sealed record ArchitectureRiskProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);