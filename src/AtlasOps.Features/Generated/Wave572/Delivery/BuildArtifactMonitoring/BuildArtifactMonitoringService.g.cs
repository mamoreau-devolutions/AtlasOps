namespace AtlasOps.Features.Delivery.BuildArtifactMonitoring;

using AtlasOps.Features;

public sealed class BuildArtifactMonitoringService(
    IAtlasOpsCapabilityRepository<BuildArtifactMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildArtifactMonitoringValidator validator = new();
    private readonly BuildArtifactMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildArtifactMonitoringChanged>> ExecuteAsync(
        UpdateBuildArtifactMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildArtifactMonitoringChanged>.Invalid(issues);
        }

        BuildArtifactMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildArtifactMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildArtifactMonitoringChanged>.Invalid(
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

        BuildArtifactMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildArtifactMonitoringChanged>.Success(changed);
    }
}