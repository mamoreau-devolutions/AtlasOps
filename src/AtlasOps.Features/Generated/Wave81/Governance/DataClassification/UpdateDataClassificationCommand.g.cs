namespace AtlasOps.Features.Governance.DataClassification;

public sealed record UpdateDataClassificationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);