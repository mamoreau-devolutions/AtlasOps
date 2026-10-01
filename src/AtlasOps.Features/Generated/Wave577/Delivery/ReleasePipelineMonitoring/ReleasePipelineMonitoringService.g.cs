namespace AtlasOps.Features.Delivery.ReleasePipelineMonitoring;

using AtlasOps.Features;

public sealed class ReleasePipelineMonitoringService(
    IAtlasOpsCapabilityRepository<ReleasePipelineMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleasePipelineMonitoringValidator validator = new();
    private readonly ReleasePipelineMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleasePipelineMonitoringChanged>> ExecuteAsync(
        UpdateReleasePipelineMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleasePipelineMonitoringChanged>.Invalid(issues);
        }

        ReleasePipelineMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleasePipelineMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleasePipelineMonitoringChanged>.Invalid(
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

        ReleasePipelineMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleasePipelineMonitoringChanged>.Success(changed);
    }
}