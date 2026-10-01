namespace AtlasOps.Features.Mobile.MobileFleetGovernance;

using AtlasOps.Features;

public sealed class MobileFleetGovernanceService(
    IAtlasOpsCapabilityRepository<MobileFleetGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileFleetGovernanceValidator validator = new();
    private readonly MobileFleetGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileFleetGovernanceChanged>> ExecuteAsync(
        UpdateMobileFleetGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileFleetGovernanceChanged>.Invalid(issues);
        }

        MobileFleetGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileFleetGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileFleetGovernanceChanged>.Invalid(
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

        MobileFleetGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileFleetGovernanceChanged>.Success(changed);
    }
}