namespace AtlasOps.Features.Cloud.AwsAccountRecovery;

public sealed record UpdateAwsAccountRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);