namespace AtlasOps.Features.Mobile.MobileProfileGovernance;

using AtlasOps.Features;

public sealed class MobileProfileGovernanceService(
    IAtlasOpsCapabilityRepository<MobileProfileGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileProfileGovernanceValidator validator = new();
    private readonly MobileProfileGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileProfileGovernanceChanged>> ExecuteAsync(
        UpdateMobileProfileGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileProfileGovernanceChanged>.Invalid(issues);
        }

        MobileProfileGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileProfileGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileProfileGovernanceChanged>.Invalid(
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

        MobileProfileGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileProfileGovernanceChanged>.Success(changed);
    }
}