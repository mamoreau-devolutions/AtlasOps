namespace AtlasOps.Features.Cloud.AzureSubscriptionGovernance;

using AtlasOps.Features;

public sealed class AzureSubscriptionGovernanceService(
    IAtlasOpsCapabilityRepository<AzureSubscriptionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly AzureSubscriptionGovernanceValidator validator = new();
    private readonly AzureSubscriptionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<AzureSubscriptionGovernanceChanged>> ExecuteAsync(
        UpdateAzureSubscriptionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AzureSubscriptionGovernanceChanged>.Invalid(issues);
        }

        AzureSubscriptionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AzureSubscriptionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AzureSubscriptionGovernanceChanged>.Invalid(
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

        AzureSubscriptionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AzureSubscriptionGovernanceChanged>.Success(changed);
    }
}