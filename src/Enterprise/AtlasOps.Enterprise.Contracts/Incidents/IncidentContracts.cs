namespace AtlasOps.Enterprise.Contracts.Incidents;

public enum IncidentSeverity
{
    Sev1,
    Sev2,
    Sev3,
    Sev4,
}

public enum IncidentState
{
    New,
    Triaged,
    Acknowledged,
    Investigating,
    Mitigated,
    Resolved,
    Closed,
    Cancelled,
}

public enum IncidentTimelineKind
{
    Created,
    StateChanged,
    AssignmentChanged,
    SeverityChanged,
    Note,
    Escalated,
    ServiceImpacted,
    MitigationApplied,
    Resolved,
}

public sealed record Incident(
    string Id,
    string Title,
    string Description,
    IncidentSeverity Severity,
    IncidentState State,
    string ServiceId,
    string Owner,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? AcknowledgedAt = null,
    DateTimeOffset? ResolvedAt = null);

public sealed record IncidentTimelineEntry(
    string Id,
    string IncidentId,
    IncidentTimelineKind Kind,
    DateTimeOffset OccurredAt,
    string Actor,
    string Message,
    IReadOnlyDictionary<string, string> Details);

public sealed record IncidentTransitionResult(
    bool Succeeded,
    Incident Incident,
    IncidentTimelineEntry? TimelineEntry,
    string Message);

public sealed record ServiceLevelTarget(
    IncidentSeverity Severity,
    TimeSpan AcknowledgementTarget,
    TimeSpan ResolutionTarget,
    TimeSpan EscalationLeadTime);

public sealed record IncidentSlaStatus(
    DateTimeOffset AcknowledgementDeadline,
    DateTimeOffset ResolutionDeadline,
    TimeSpan AcknowledgementRemaining,
    TimeSpan ResolutionRemaining,
    bool IsAcknowledgementBreached,
    bool IsResolutionBreached,
    double ResolutionBudgetConsumedPercent);

public sealed record EscalationRecommendation(
    int Level,
    bool ShouldEscalate,
    string Target,
    string Reason,
    DateTimeOffset EvaluatedAt);

public sealed record OnCallShift(
    string Id,
    string Team,
    string Primary,
    string? Secondary,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt);

public sealed record ServiceDependency(
    string ServiceId,
    string DependsOnServiceId,
    bool IsCritical,
    string Relationship);

public sealed record ServiceImpact(
    string ServiceId,
    int Distance,
    bool IsCriticalPath,
    IReadOnlyList<string> Path);
