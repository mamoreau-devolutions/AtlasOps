namespace AtlasOps.Features.Architecture.ArchitectureDecisionProvisioning;

public sealed record ArchitectureDecisionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);