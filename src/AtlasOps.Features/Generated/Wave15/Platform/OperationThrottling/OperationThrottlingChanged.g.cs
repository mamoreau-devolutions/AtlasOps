namespace AtlasOps.Features.Platform.OperationThrottling;

public sealed record OperationThrottlingChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);