namespace AtlasOps.Features.Compute.ComputeScaleSetRecovery;

using AtlasOps.Features;

public sealed class ComputeScaleSetRecoveryService(
    IAtlasOpsCapabilityRepository<ComputeScaleSetRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeScaleSetRecoveryValidator validator = new();
    private readonly ComputeScaleSetRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeScaleSetRecoveryChanged>> ExecuteAsync(
        UpdateComputeScaleSetRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeScaleSetRecoveryChanged>.Invalid(issues);
        }

        ComputeScaleSetRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeScaleSetRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeScaleSetRecoveryChanged>.Invalid(
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

        ComputeScaleSetRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeScaleSetRecoveryChanged>.Success(changed);
    }
}