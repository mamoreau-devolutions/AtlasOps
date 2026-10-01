namespace AtlasOps.Features.Observability.LogQueryProvisioning;

using AtlasOps.Features;

public sealed class LogQueryProvisioningService(
    IAtlasOpsCapabilityRepository<LogQueryProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogQueryProvisioningValidator validator = new();
    private readonly LogQueryProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogQueryProvisioningChanged>> ExecuteAsync(
        UpdateLogQueryProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogQueryProvisioningChanged>.Invalid(issues);
        }

        LogQueryProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogQueryProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogQueryProvisioningChanged>.Invalid(
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

        LogQueryProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogQueryProvisioningChanged>.Success(changed);
    }
}