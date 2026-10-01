namespace AtlasOps.Features.Architecture.ArchitectureEvidenceProvisioning;

public sealed record ArchitectureEvidenceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);