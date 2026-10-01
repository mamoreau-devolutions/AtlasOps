namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleRecovery;

public sealed record UpdateKnowledgeArticleRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);