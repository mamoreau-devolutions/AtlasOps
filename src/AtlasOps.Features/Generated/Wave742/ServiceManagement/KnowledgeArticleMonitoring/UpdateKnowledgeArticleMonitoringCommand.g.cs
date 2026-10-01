namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleMonitoring;

public sealed record UpdateKnowledgeArticleMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);