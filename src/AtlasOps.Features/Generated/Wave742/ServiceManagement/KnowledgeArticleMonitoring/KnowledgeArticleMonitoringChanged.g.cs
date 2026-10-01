namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleMonitoring;

public sealed record KnowledgeArticleMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);