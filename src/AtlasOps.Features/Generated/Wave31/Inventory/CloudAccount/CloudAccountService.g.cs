namespace AtlasOps.Features.Inventory.CloudAccount;

using AtlasOps.Features;

public sealed class CloudAccountService(
    IAtlasOpsCapabilityRepository<CloudAccountItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudAccountValidator validator = new();
    private readonly CloudAccountPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudAccountChanged>> ExecuteAsync(
        UpdateCloudAccountCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudAccountChanged>.Invalid(issues);
        }

        CloudAccountItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudAccountItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudAccountChanged>.Invalid(
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

        CloudAccountChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudAccountChanged>.Success(changed);
    }
}