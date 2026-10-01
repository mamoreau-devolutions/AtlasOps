namespace AtlasOps.Features.Hardening.UpgradeAssessment;

public sealed record UpdateUpgradeAssessmentCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);