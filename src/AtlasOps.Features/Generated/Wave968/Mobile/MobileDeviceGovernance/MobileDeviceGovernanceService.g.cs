namespace AtlasOps.Features.Mobile.MobileDeviceGovernance;

using AtlasOps.Features;

public sealed class MobileDeviceGovernanceService(
    IAtlasOpsCapabilityRepository<MobileDeviceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileDeviceGovernanceValidator validator = new();
    private readonly MobileDeviceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileDeviceGovernanceChanged>> ExecuteAsync(
        UpdateMobileDeviceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileDeviceGovernanceChanged>.Invalid(issues);
        }

        MobileDeviceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileDeviceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileDeviceGovernanceChanged>.Invalid(
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

        MobileDeviceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileDeviceGovernanceChanged>.Success(changed);
    }
}