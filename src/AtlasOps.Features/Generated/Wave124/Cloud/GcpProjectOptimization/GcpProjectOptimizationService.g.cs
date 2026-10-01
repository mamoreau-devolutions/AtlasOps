namespace AtlasOps.Features.Cloud.GcpProjectOptimization;

using AtlasOps.Features;

public sealed class GcpProjectOptimizationService(
    IAtlasOpsCapabilityRepository<GcpProjectOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly GcpProjectOptimizationValidator validator = new();
    private readonly GcpProjectOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<GcpProjectOptimizationChanged>> ExecuteAsync(
        UpdateGcpProjectOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<GcpProjectOptimizationChanged>.Invalid(issues);
        }

        GcpProjectOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new GcpProjectOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<GcpProjectOptimizationChanged>.Invalid(
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

        GcpProjectOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<GcpProjectOptimizationChanged>.Success(changed);
    }
}