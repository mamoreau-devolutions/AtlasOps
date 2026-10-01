namespace AtlasOps.Features.Compute.ComputeTemplateMonitoring;

using AtlasOps.Features;

public sealed class ComputeTemplateMonitoringService(
    IAtlasOpsCapabilityRepository<ComputeTemplateMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeTemplateMonitoringValidator validator = new();
    private readonly ComputeTemplateMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeTemplateMonitoringChanged>> ExecuteAsync(
        UpdateComputeTemplateMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeTemplateMonitoringChanged>.Invalid(issues);
        }

        ComputeTemplateMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeTemplateMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeTemplateMonitoringChanged>.Invalid(
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

        ComputeTemplateMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeTemplateMonitoringChanged>.Success(changed);
    }
}