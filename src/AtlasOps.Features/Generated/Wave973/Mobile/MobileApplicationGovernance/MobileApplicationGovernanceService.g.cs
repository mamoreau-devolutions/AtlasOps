namespace AtlasOps.Features.Mobile.MobileApplicationGovernance;

using AtlasOps.Features;

public sealed class MobileApplicationGovernanceService(
    IAtlasOpsCapabilityRepository<MobileApplicationGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileApplicationGovernanceValidator validator = new();
    private readonly MobileApplicationGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileApplicationGovernanceChanged>> ExecuteAsync(
        UpdateMobileApplicationGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileApplicationGovernanceChanged>.Invalid(issues);
        }

        MobileApplicationGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileApplicationGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileApplicationGovernanceChanged>.Invalid(
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

        MobileApplicationGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileApplicationGovernanceChanged>.Success(changed);
    }
}