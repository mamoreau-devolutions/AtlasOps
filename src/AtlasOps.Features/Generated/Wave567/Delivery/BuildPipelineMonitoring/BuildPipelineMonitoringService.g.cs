namespace AtlasOps.Features.Delivery.BuildPipelineMonitoring;

using AtlasOps.Features;

public sealed class BuildPipelineMonitoringService(
    IAtlasOpsCapabilityRepository<BuildPipelineMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildPipelineMonitoringValidator validator = new();
    private readonly BuildPipelineMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildPipelineMonitoringChanged>> ExecuteAsync(
        UpdateBuildPipelineMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildPipelineMonitoringChanged>.Invalid(issues);
        }

        BuildPipelineMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildPipelineMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildPipelineMonitoringChanged>.Invalid(
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

        BuildPipelineMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildPipelineMonitoringChanged>.Success(changed);
    }
}