namespace AtlasOps.Features.Automation.ApprovalRequest;

public sealed record UpdateApprovalRequestCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);