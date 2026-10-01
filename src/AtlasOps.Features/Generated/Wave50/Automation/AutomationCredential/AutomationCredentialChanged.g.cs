namespace AtlasOps.Features.Automation.AutomationCredential;

public sealed record AutomationCredentialChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);