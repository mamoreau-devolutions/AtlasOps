namespace AtlasOps.Features.Delivery.SourceRepositoryProvisioning;

using AtlasOps.Features;

public sealed class SourceRepositoryProvisioningService(
    IAtlasOpsCapabilityRepository<SourceRepositoryProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SourceRepositoryProvisioningValidator validator = new();
    private readonly SourceRepositoryProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SourceRepositoryProvisioningChanged>> ExecuteAsync(
        UpdateSourceRepositoryProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SourceRepositoryProvisioningChanged>.Invalid(issues);
        }

        SourceRepositoryProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SourceRepositoryProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SourceRepositoryProvisioningChanged>.Invalid(
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

        SourceRepositoryProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SourceRepositoryProvisioningChanged>.Success(changed);
    }
}