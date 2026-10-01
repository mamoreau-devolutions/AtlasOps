namespace AtlasOps.Features.Hardening.UpgradeChannel;

using AtlasOps.Features;

public sealed class UpgradeChannelService(
    IAtlasOpsCapabilityRepository<UpgradeChannelItem> repository,
    TimeProvider timeProvider)
{
    private readonly UpgradeChannelValidator validator = new();
    private readonly UpgradeChannelPolicy policy = new();

    public async Task<AtlasOpsOperationResult<UpgradeChannelChanged>> ExecuteAsync(
        UpdateUpgradeChannelCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<UpgradeChannelChanged>.Invalid(issues);
        }

        UpgradeChannelItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new UpgradeChannelItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<UpgradeChannelChanged>.Invalid(
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

        UpgradeChannelChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<UpgradeChannelChanged>.Success(changed);
    }
}