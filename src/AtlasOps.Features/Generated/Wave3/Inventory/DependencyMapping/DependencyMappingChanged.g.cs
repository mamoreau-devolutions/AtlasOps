namespace AtlasOps.Features.Inventory.DependencyMapping;

public sealed record DependencyMappingChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);