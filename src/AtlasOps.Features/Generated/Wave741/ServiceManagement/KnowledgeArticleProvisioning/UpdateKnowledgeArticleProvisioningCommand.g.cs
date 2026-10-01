namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleProvisioning;

public sealed record UpdateKnowledgeArticleProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);