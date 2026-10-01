namespace AtlasOps.Features.Cloud.AwsAccountGovernance;

using AtlasOps.Features;

public sealed class AwsAccountGovernanceService(
    IAtlasOpsCapabilityRepository<AwsAccountGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly AwsAccountGovernanceValidator validator = new();
    private readonly AwsAccountGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<AwsAccountGovernanceChanged>> ExecuteAsync(
        UpdateAwsAccountGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AwsAccountGovernanceChanged>.Invalid(issues);
        }

        AwsAccountGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AwsAccountGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AwsAccountGovernanceChanged>.Invalid(
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

        AwsAccountGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AwsAccountGovernanceChanged>.Success(changed);
    }
}