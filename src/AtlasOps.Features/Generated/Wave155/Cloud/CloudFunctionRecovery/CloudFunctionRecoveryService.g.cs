namespace AtlasOps.Features.Cloud.CloudFunctionRecovery;

using AtlasOps.Features;

public sealed class CloudFunctionRecoveryService(
    IAtlasOpsCapabilityRepository<CloudFunctionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudFunctionRecoveryValidator validator = new();
    private readonly CloudFunctionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudFunctionRecoveryChanged>> ExecuteAsync(
        UpdateCloudFunctionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudFunctionRecoveryChanged>.Invalid(issues);
        }

        CloudFunctionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudFunctionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudFunctionRecoveryChanged>.Invalid(
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

        CloudFunctionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudFunctionRecoveryChanged>.Success(changed);
    }
}