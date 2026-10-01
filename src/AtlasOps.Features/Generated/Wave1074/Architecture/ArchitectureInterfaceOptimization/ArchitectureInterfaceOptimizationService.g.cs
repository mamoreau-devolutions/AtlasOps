namespace AtlasOps.Features.Architecture.ArchitectureInterfaceOptimization;

using AtlasOps.Features;

public sealed class ArchitectureInterfaceOptimizationService(
    IAtlasOpsCapabilityRepository<ArchitectureInterfaceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureInterfaceOptimizationValidator validator = new();
    private readonly ArchitectureInterfaceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureInterfaceOptimizationChanged>> ExecuteAsync(
        UpdateArchitectureInterfaceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureInterfaceOptimizationChanged>.Invalid(issues);
        }

        ArchitectureInterfaceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureInterfaceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureInterfaceOptimizationChanged>.Invalid(
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

        ArchitectureInterfaceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureInterfaceOptimizationChanged>.Success(changed);
    }
}