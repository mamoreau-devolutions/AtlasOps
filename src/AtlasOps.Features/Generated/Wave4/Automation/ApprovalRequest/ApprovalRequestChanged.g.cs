namespace AtlasOps.Features.Automation.ApprovalRequest;

public sealed record ApprovalRequestChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);