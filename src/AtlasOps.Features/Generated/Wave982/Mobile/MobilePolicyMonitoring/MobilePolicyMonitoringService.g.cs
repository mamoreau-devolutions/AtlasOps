namespace AtlasOps.Features.Mobile.MobilePolicyMonitoring;

using AtlasOps.Features;

public sealed class MobilePolicyMonitoringService(
    IAtlasOpsCapabilityRepository<MobilePolicyMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobilePolicyMonitoringValidator validator = new();
    private readonly MobilePolicyMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobilePolicyMonitoringChanged>> ExecuteAsync(
        UpdateMobilePolicyMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobilePolicyMonitoringChanged>.Invalid(issues);
        }

        MobilePolicyMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobilePolicyMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobilePolicyMonitoringChanged>.Invalid(
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

        MobilePolicyMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobilePolicyMonitoringChanged>.Success(changed);
    }
}