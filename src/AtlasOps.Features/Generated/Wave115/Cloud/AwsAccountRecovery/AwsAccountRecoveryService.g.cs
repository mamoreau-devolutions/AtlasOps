namespace AtlasOps.Features.Cloud.AwsAccountRecovery;

using AtlasOps.Features;

public sealed class AwsAccountRecoveryService(
    IAtlasOpsCapabilityRepository<AwsAccountRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly AwsAccountRecoveryValidator validator = new();
    private readonly AwsAccountRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AwsAccountRecoveryChanged>> ExecuteAsync(
        UpdateAwsAccountRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AwsAccountRecoveryChanged>.Invalid(issues);
        }

        AwsAccountRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AwsAccountRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AwsAccountRecoveryChanged>.Invalid(
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

        AwsAccountRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AwsAccountRecoveryChanged>.Success(changed);
    }
}