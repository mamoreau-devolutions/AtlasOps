namespace AtlasOps.Features.Platform.ModuleIsolation;

public sealed record ModuleIsolationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);