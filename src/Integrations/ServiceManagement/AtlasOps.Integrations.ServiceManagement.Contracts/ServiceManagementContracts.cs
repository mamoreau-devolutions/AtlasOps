namespace AtlasOps.Integrations.ServiceManagement.Contracts;

public sealed record ServiceTicket(
    string ProviderId,
    string TicketId,
    string State,
    string Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ResolvedAt,
    TimeSpan AccumulatedPausedTime,
    DateTimeOffset? PausedAt,
    string Revision);

public sealed record TicketTransition(
    string Name,
    IReadOnlySet<string> FromStates,
    string TargetState,
    bool PausesServiceLevel,
    bool ResolvesTicket);

public sealed record ServiceLevelPolicy(
    string Priority,
    TimeSpan ResponseTarget,
    TimeSpan ResolutionTarget,
    IReadOnlyList<TimeSpan> EscalationThresholds);

public sealed record ServiceLevelEvaluation(
    TimeSpan ActiveAge,
    TimeSpan Remaining,
    int EscalationLevel,
    bool Breached,
    IReadOnlyList<string> Diagnostics);
