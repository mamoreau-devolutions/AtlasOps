namespace AtlasOps.Features.Incidents.EscalationPolicy;

public sealed record EscalationPolicyChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);