namespace AtlasOps.Features.Cloud.CloudDatabaseGovernance;

using AtlasOps.Features;

public sealed class CloudDatabaseGovernanceService(
    IAtlasOpsCapabilityRepository<CloudDatabaseGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudDatabaseGovernanceValidator validator = new();
    private readonly CloudDatabaseGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudDatabaseGovernanceChanged>> ExecuteAsync(
        UpdateCloudDatabaseGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudDatabaseGovernanceChanged>.Invalid(issues);
        }

        CloudDatabaseGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudDatabaseGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudDatabaseGovernanceChanged>.Invalid(
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

        CloudDatabaseGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudDatabaseGovernanceChanged>.Success(changed);
    }
}