namespace AtlasOps.Features.Cloud.AzureSubscriptionRecovery;

using AtlasOps.Features;

public sealed class AzureSubscriptionRecoveryService(
    IAtlasOpsCapabilityRepository<AzureSubscriptionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly AzureSubscriptionRecoveryValidator validator = new();
    private readonly AzureSubscriptionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AzureSubscriptionRecoveryChanged>> ExecuteAsync(
        UpdateAzureSubscriptionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AzureSubscriptionRecoveryChanged>.Invalid(issues);
        }

        AzureSubscriptionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AzureSubscriptionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AzureSubscriptionRecoveryChanged>.Invalid(
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

        AzureSubscriptionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AzureSubscriptionRecoveryChanged>.Success(changed);
    }
}