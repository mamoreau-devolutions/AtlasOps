namespace AtlasOps.Features.Delivery.SourceRepositoryOptimization;

using AtlasOps.Features;

public sealed class SourceRepositoryOptimizationService(
    IAtlasOpsCapabilityRepository<SourceRepositoryOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SourceRepositoryOptimizationValidator validator = new();
    private readonly SourceRepositoryOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SourceRepositoryOptimizationChanged>> ExecuteAsync(
        UpdateSourceRepositoryOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SourceRepositoryOptimizationChanged>.Invalid(issues);
        }

        SourceRepositoryOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SourceRepositoryOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SourceRepositoryOptimizationChanged>.Invalid(
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

        SourceRepositoryOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SourceRepositoryOptimizationChanged>.Success(changed);
    }
}