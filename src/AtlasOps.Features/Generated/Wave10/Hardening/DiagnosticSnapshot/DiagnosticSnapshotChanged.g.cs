namespace AtlasOps.Features.Hardening.DiagnosticSnapshot;

public sealed record DiagnosticSnapshotChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);