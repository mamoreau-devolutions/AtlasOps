namespace AtlasOps.Features.Governance.RetentionPolicy;

using AtlasOps.Features;

public sealed class RetentionPolicyService(
    IAtlasOpsCapabilityRepository<RetentionPolicyItem> repository,
    TimeProvider timeProvider)
{
    private readonly RetentionPolicyValidator validator = new();
    private readonly RetentionPolicyPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RetentionPolicyChanged>> ExecuteAsync(
        UpdateRetentionPolicyCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RetentionPolicyChanged>.Invalid(issues);
        }

        RetentionPolicyItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RetentionPolicyItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RetentionPolicyChanged>.Invalid(
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

        RetentionPolicyChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RetentionPolicyChanged>.Success(changed);
    }
}