namespace AtlasOps.Features.Hardening.DiagnosticSnapshot;

public sealed record UpdateDiagnosticSnapshotCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);