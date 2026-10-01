namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleOptimization;

using AtlasOps.Features;

public sealed class KnowledgeArticleOptimizationService(
    IAtlasOpsCapabilityRepository<KnowledgeArticleOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KnowledgeArticleOptimizationValidator validator = new();
    private readonly KnowledgeArticleOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KnowledgeArticleOptimizationChanged>> ExecuteAsync(
        UpdateKnowledgeArticleOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KnowledgeArticleOptimizationChanged>.Invalid(issues);
        }

        KnowledgeArticleOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KnowledgeArticleOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KnowledgeArticleOptimizationChanged>.Invalid(
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

        KnowledgeArticleOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KnowledgeArticleOptimizationChanged>.Success(changed);
    }
}