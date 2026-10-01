namespace AtlasOps.Features.Architecture.ArchitectureRoadmapProvisioning;

public sealed record ArchitectureRoadmapProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);