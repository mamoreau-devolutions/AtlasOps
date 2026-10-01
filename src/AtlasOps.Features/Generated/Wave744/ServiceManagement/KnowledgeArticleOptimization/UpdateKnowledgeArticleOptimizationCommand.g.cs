namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleOptimization;

public sealed record UpdateKnowledgeArticleOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);