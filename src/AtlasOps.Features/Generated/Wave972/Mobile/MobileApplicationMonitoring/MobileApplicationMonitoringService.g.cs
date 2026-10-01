namespace AtlasOps.Features.Mobile.MobileApplicationMonitoring;

using AtlasOps.Features;

public sealed class MobileApplicationMonitoringService(
    IAtlasOpsCapabilityRepository<MobileApplicationMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileApplicationMonitoringValidator validator = new();
    private readonly MobileApplicationMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileApplicationMonitoringChanged>> ExecuteAsync(
        UpdateMobileApplicationMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileApplicationMonitoringChanged>.Invalid(issues);
        }

        MobileApplicationMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileApplicationMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileApplicationMonitoringChanged>.Invalid(
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

        MobileApplicationMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileApplicationMonitoringChanged>.Success(changed);
    }
}