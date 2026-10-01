namespace AtlasOps.Features.Architecture.ArchitectureReviewMonitoring;

using AtlasOps.Features;

public sealed class ArchitectureReviewMonitoringService(
    IAtlasOpsCapabilityRepository<ArchitectureReviewMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureReviewMonitoringValidator validator = new();
    private readonly ArchitectureReviewMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureReviewMonitoringChanged>> ExecuteAsync(
        UpdateArchitectureReviewMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureReviewMonitoringChanged>.Invalid(issues);
        }

        ArchitectureReviewMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureReviewMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureReviewMonitoringChanged>.Invalid(
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

        ArchitectureReviewMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureReviewMonitoringChanged>.Success(changed);
    }
}