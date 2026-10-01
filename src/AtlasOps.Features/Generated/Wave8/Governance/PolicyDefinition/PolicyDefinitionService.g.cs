namespace AtlasOps.Features.Governance.PolicyDefinition;

using AtlasOps.Features;

public sealed class PolicyDefinitionService(
    IAtlasOpsCapabilityRepository<PolicyDefinitionItem> repository,
    TimeProvider timeProvider)
{
    private readonly PolicyDefinitionValidator validator = new();
    private readonly PolicyDefinitionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<PolicyDefinitionChanged>> ExecuteAsync(
        UpdatePolicyDefinitionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<PolicyDefinitionChanged>.Invalid(issues);
        }

        PolicyDefinitionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new PolicyDefinitionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<PolicyDefinitionChanged>.Invalid(
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

        PolicyDefinitionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<PolicyDefinitionChanged>.Success(changed);
    }
}