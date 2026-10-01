namespace AtlasOps.Features.Mobile.MobileUpdateGovernance;

using AtlasOps.Features;

public sealed class MobileUpdateGovernanceService(
    IAtlasOpsCapabilityRepository<MobileUpdateGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileUpdateGovernanceValidator validator = new();
    private readonly MobileUpdateGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileUpdateGovernanceChanged>> ExecuteAsync(
        UpdateMobileUpdateGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileUpdateGovernanceChanged>.Invalid(issues);
        }

        MobileUpdateGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileUpdateGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileUpdateGovernanceChanged>.Invalid(
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

        MobileUpdateGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileUpdateGovernanceChanged>.Success(changed);
    }
}