namespace AtlasOps.Features.Mobile.MobileDeviceRecovery;

using AtlasOps.Features;

public sealed class MobileDeviceRecoveryService(
    IAtlasOpsCapabilityRepository<MobileDeviceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileDeviceRecoveryValidator validator = new();
    private readonly MobileDeviceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileDeviceRecoveryChanged>> ExecuteAsync(
        UpdateMobileDeviceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileDeviceRecoveryChanged>.Invalid(issues);
        }

        MobileDeviceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileDeviceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileDeviceRecoveryChanged>.Invalid(
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

        MobileDeviceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileDeviceRecoveryChanged>.Success(changed);
    }
}