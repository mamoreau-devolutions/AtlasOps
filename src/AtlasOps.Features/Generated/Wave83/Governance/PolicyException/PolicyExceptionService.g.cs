namespace AtlasOps.Features.Governance.PolicyException;

using AtlasOps.Features;

public sealed class PolicyExceptionService(
    IAtlasOpsCapabilityRepository<PolicyExceptionItem> repository,
    TimeProvider timeProvider)
{
    private readonly PolicyExceptionValidator validator = new();
    private readonly PolicyExceptionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<PolicyExceptionChanged>> ExecuteAsync(
        UpdatePolicyExceptionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<PolicyExceptionChanged>.Invalid(issues);
        }

        PolicyExceptionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new PolicyExceptionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<PolicyExceptionChanged>.Invalid(
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

        PolicyExceptionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<PolicyExceptionChanged>.Success(changed);
    }
}