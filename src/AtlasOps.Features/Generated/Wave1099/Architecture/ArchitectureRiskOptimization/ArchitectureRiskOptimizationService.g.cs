namespace AtlasOps.Features.Architecture.ArchitectureRiskOptimization;

using AtlasOps.Features;

public sealed class ArchitectureRiskOptimizationService(
    IAtlasOpsCapabilityRepository<ArchitectureRiskOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRiskOptimizationValidator validator = new();
    private readonly ArchitectureRiskOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRiskOptimizationChanged>> ExecuteAsync(
        UpdateArchitectureRiskOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRiskOptimizationChanged>.Invalid(issues);
        }

        ArchitectureRiskOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRiskOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRiskOptimizationChanged>.Invalid(
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

        ArchitectureRiskOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRiskOptimizationChanged>.Success(changed);
    }
}