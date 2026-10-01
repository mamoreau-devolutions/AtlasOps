namespace AtlasOps.Features.Governance.LegalHold;

public sealed record LegalHoldChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);