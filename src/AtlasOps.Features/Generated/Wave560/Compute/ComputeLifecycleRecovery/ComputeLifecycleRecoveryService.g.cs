namespace AtlasOps.Features.Compute.ComputeLifecycleRecovery;

using AtlasOps.Features;

public sealed class ComputeLifecycleRecoveryService(
    IAtlasOpsCapabilityRepository<ComputeLifecycleRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeLifecycleRecoveryValidator validator = new();
    private readonly ComputeLifecycleRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeLifecycleRecoveryChanged>> ExecuteAsync(
        UpdateComputeLifecycleRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeLifecycleRecoveryChanged>.Invalid(issues);
        }

        ComputeLifecycleRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeLifecycleRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeLifecycleRecoveryChanged>.Invalid(
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

        ComputeLifecycleRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeLifecycleRecoveryChanged>.Success(changed);
    }
}