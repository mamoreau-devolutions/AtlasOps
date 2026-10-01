namespace AtlasOps.Features.Compute.ComputeConsoleOptimization;

using AtlasOps.Features;

public sealed class ComputeConsoleOptimizationService(
    IAtlasOpsCapabilityRepository<ComputeConsoleOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeConsoleOptimizationValidator validator = new();
    private readonly ComputeConsoleOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeConsoleOptimizationChanged>> ExecuteAsync(
        UpdateComputeConsoleOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeConsoleOptimizationChanged>.Invalid(issues);
        }

        ComputeConsoleOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeConsoleOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeConsoleOptimizationChanged>.Invalid(
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

        ComputeConsoleOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeConsoleOptimizationChanged>.Success(changed);
    }
}