namespace AtlasOps.Features.Governance.OwnershipRule;

using AtlasOps.Features;

public sealed class OwnershipRuleService(
    IAtlasOpsCapabilityRepository<OwnershipRuleItem> repository,
    TimeProvider timeProvider)
{
    private readonly OwnershipRuleValidator validator = new();
    private readonly OwnershipRulePolicy policy = new();

    public async Task<AtlasOpsOperationResult<OwnershipRuleChanged>> ExecuteAsync(
        UpdateOwnershipRuleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<OwnershipRuleChanged>.Invalid(issues);
        }

        OwnershipRuleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new OwnershipRuleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<OwnershipRuleChanged>.Invalid(
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

        OwnershipRuleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<OwnershipRuleChanged>.Success(changed);
    }
}