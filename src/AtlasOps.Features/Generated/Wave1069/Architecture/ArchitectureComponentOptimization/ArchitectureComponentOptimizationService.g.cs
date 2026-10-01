namespace AtlasOps.Features.Architecture.ArchitectureComponentOptimization;

using AtlasOps.Features;

public sealed class ArchitectureComponentOptimizationService(
    IAtlasOpsCapabilityRepository<ArchitectureComponentOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureComponentOptimizationValidator validator = new();
    private readonly ArchitectureComponentOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureComponentOptimizationChanged>> ExecuteAsync(
        UpdateArchitectureComponentOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureComponentOptimizationChanged>.Invalid(issues);
        }

        ArchitectureComponentOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureComponentOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureComponentOptimizationChanged>.Invalid(
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

        ArchitectureComponentOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureComponentOptimizationChanged>.Success(changed);
    }
}