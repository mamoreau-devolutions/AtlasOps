namespace AtlasOps.Features.Architecture.ArchitectureDependencyOptimization;

using AtlasOps.Features;

public sealed class ArchitectureDependencyOptimizationService(
    IAtlasOpsCapabilityRepository<ArchitectureDependencyOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDependencyOptimizationValidator validator = new();
    private readonly ArchitectureDependencyOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDependencyOptimizationChanged>> ExecuteAsync(
        UpdateArchitectureDependencyOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDependencyOptimizationChanged>.Invalid(issues);
        }

        ArchitectureDependencyOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDependencyOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDependencyOptimizationChanged>.Invalid(
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

        ArchitectureDependencyOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDependencyOptimizationChanged>.Success(changed);
    }
}