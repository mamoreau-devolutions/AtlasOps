namespace AtlasOps.Features.Observability.ObservabilityDashboardRecovery;

using AtlasOps.Features;

public sealed class ObservabilityDashboardRecoveryService(
    IAtlasOpsCapabilityRepository<ObservabilityDashboardRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityDashboardRecoveryValidator validator = new();
    private readonly ObservabilityDashboardRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityDashboardRecoveryChanged>> ExecuteAsync(
        UpdateObservabilityDashboardRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityDashboardRecoveryChanged>.Invalid(issues);
        }

        ObservabilityDashboardRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityDashboardRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityDashboardRecoveryChanged>.Invalid(
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

        ObservabilityDashboardRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityDashboardRecoveryChanged>.Success(changed);
    }
}