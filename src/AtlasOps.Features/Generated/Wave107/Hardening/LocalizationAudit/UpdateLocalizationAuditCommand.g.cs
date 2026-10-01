namespace AtlasOps.Features.Hardening.LocalizationAudit;

public sealed record UpdateLocalizationAuditCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);