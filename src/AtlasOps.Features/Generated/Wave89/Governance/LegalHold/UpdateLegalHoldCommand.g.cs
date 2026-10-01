namespace AtlasOps.Features.Governance.LegalHold;

public sealed record UpdateLegalHoldCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);