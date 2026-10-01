namespace AtlasOps.Features.Cloud.CloudIdentityRecovery;

using AtlasOps.Features;

public sealed class CloudIdentityRecoveryService(
    IAtlasOpsCapabilityRepository<CloudIdentityRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudIdentityRecoveryValidator validator = new();
    private readonly CloudIdentityRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudIdentityRecoveryChanged>> ExecuteAsync(
        UpdateCloudIdentityRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudIdentityRecoveryChanged>.Invalid(issues);
        }

        CloudIdentityRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudIdentityRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudIdentityRecoveryChanged>.Invalid(
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

        CloudIdentityRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudIdentityRecoveryChanged>.Success(changed);
    }
}