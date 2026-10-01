namespace AtlasOps.Features.Architecture.ArchitectureStandardOptimization;

using AtlasOps.Features;

public sealed class ArchitectureStandardOptimizationService(
    IAtlasOpsCapabilityRepository<ArchitectureStandardOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureStandardOptimizationValidator validator = new();
    private readonly ArchitectureStandardOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureStandardOptimizationChanged>> ExecuteAsync(
        UpdateArchitectureStandardOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureStandardOptimizationChanged>.Invalid(issues);
        }

        ArchitectureStandardOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureStandardOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureStandardOptimizationChanged>.Invalid(
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

        ArchitectureStandardOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureStandardOptimizationChanged>.Success(changed);
    }
}