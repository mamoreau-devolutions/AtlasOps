namespace AtlasOps.Features.Compute.ComputeMetricRecovery;

using AtlasOps.Features;

public sealed class ComputeMetricRecoveryService(
    IAtlasOpsCapabilityRepository<ComputeMetricRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeMetricRecoveryValidator validator = new();
    private readonly ComputeMetricRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeMetricRecoveryChanged>> ExecuteAsync(
        UpdateComputeMetricRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeMetricRecoveryChanged>.Invalid(issues);
        }

        ComputeMetricRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeMetricRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeMetricRecoveryChanged>.Invalid(
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

        ComputeMetricRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeMetricRecoveryChanged>.Success(changed);
    }
}