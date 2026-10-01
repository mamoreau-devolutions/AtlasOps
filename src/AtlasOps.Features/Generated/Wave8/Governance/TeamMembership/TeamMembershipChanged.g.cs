namespace AtlasOps.Features.Governance.TeamMembership;

public sealed record TeamMembershipChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);