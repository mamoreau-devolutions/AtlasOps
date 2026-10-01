namespace AtlasOps.Features.Architecture.ArchitectureReviewOptimization;

using AtlasOps.Features;

public sealed class ArchitectureReviewOptimizationService(
    IAtlasOpsCapabilityRepository<ArchitectureReviewOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureReviewOptimizationValidator validator = new();
    private readonly ArchitectureReviewOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureReviewOptimizationChanged>> ExecuteAsync(
        UpdateArchitectureReviewOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureReviewOptimizationChanged>.Invalid(issues);
        }

        ArchitectureReviewOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureReviewOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureReviewOptimizationChanged>.Invalid(
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

        ArchitectureReviewOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureReviewOptimizationChanged>.Success(changed);
    }
}