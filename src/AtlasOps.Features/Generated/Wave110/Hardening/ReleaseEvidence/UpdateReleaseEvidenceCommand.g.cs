namespace AtlasOps.Features.Hardening.ReleaseEvidence;

public sealed record UpdateReleaseEvidenceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);