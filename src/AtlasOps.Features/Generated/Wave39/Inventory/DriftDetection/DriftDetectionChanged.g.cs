namespace AtlasOps.Features.Inventory.DriftDetection;

public sealed record DriftDetectionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);