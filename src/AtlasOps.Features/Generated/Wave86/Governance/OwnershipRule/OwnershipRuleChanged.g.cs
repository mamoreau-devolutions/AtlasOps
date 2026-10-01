namespace AtlasOps.Features.Governance.OwnershipRule;

public sealed record OwnershipRuleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);