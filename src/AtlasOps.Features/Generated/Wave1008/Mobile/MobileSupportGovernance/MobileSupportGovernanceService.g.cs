namespace AtlasOps.Features.Mobile.MobileSupportGovernance;

using AtlasOps.Features;

public sealed class MobileSupportGovernanceService(
    IAtlasOpsCapabilityRepository<MobileSupportGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileSupportGovernanceValidator validator = new();
    private readonly MobileSupportGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileSupportGovernanceChanged>> ExecuteAsync(
        UpdateMobileSupportGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileSupportGovernanceChanged>.Invalid(issues);
        }

        MobileSupportGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileSupportGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileSupportGovernanceChanged>.Invalid(
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

        MobileSupportGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileSupportGovernanceChanged>.Success(changed);
    }
}