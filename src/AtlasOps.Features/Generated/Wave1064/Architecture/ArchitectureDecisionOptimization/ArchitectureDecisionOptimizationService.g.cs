namespace AtlasOps.Features.Architecture.ArchitectureDecisionOptimization;

using AtlasOps.Features;

public sealed class ArchitectureDecisionOptimizationService(
    IAtlasOpsCapabilityRepository<ArchitectureDecisionOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDecisionOptimizationValidator validator = new();
    private readonly ArchitectureDecisionOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDecisionOptimizationChanged>> ExecuteAsync(
        UpdateArchitectureDecisionOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDecisionOptimizationChanged>.Invalid(issues);
        }

        ArchitectureDecisionOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDecisionOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDecisionOptimizationChanged>.Invalid(
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

        ArchitectureDecisionOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDecisionOptimizationChanged>.Success(changed);
    }
}