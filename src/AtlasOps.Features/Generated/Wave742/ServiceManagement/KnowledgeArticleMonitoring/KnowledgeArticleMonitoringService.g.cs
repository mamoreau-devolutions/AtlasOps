namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleMonitoring;

using AtlasOps.Features;

public sealed class KnowledgeArticleMonitoringService(
    IAtlasOpsCapabilityRepository<KnowledgeArticleMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KnowledgeArticleMonitoringValidator validator = new();
    private readonly KnowledgeArticleMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KnowledgeArticleMonitoringChanged>> ExecuteAsync(
        UpdateKnowledgeArticleMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KnowledgeArticleMonitoringChanged>.Invalid(issues);
        }

        KnowledgeArticleMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KnowledgeArticleMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KnowledgeArticleMonitoringChanged>.Invalid(
            [
                new("State", $"Cannot transition from '{previousState}' to '{command.TargetState}'."),
            ]);
        }

        entity.Name = command.Name.Trim();
        entity.Owner = command.Owner.Trim();
        entity.State = command.TargetState;
        entity.Priority = command.Priority;
        entity.IsEnabled = command.IsEnabled;
        DateTimeOffset now = timeProvider.GetUtcNow();
        entity.MarkUpdated(now);
        await repository.SaveAsync(entity, cancellationToken);

        KnowledgeArticleMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KnowledgeArticleMonitoringChanged>.Success(changed);
    }
}