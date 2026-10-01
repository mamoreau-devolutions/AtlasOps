namespace AtlasOps.Features.Delivery.ReleaseMetricRecovery;

using AtlasOps.Features;

public sealed class ReleaseMetricRecoveryService(
    IAtlasOpsCapabilityRepository<ReleaseMetricRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseMetricRecoveryValidator validator = new();
    private readonly ReleaseMetricRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseMetricRecoveryChanged>> ExecuteAsync(
        UpdateReleaseMetricRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseMetricRecoveryChanged>.Invalid(issues);
        }

        ReleaseMetricRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseMetricRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseMetricRecoveryChanged>.Invalid(
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

        ReleaseMetricRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseMetricRecoveryChanged>.Success(changed);
    }
}