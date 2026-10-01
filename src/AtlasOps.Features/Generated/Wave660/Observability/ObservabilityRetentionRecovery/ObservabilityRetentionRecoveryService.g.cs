namespace AtlasOps.Features.Observability.ObservabilityRetentionRecovery;

using AtlasOps.Features;

public sealed class ObservabilityRetentionRecoveryService(
    IAtlasOpsCapabilityRepository<ObservabilityRetentionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityRetentionRecoveryValidator validator = new();
    private readonly ObservabilityRetentionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityRetentionRecoveryChanged>> ExecuteAsync(
        UpdateObservabilityRetentionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityRetentionRecoveryChanged>.Invalid(issues);
        }

        ObservabilityRetentionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityRetentionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityRetentionRecoveryChanged>.Invalid(
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

        ObservabilityRetentionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityRetentionRecoveryChanged>.Success(changed);
    }
}