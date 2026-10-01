namespace AtlasOps.Features.Observability.TraceSourceProvisioning;

using AtlasOps.Features;

public sealed class TraceSourceProvisioningService(
    IAtlasOpsCapabilityRepository<TraceSourceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly TraceSourceProvisioningValidator validator = new();
    private readonly TraceSourceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TraceSourceProvisioningChanged>> ExecuteAsync(
        UpdateTraceSourceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TraceSourceProvisioningChanged>.Invalid(issues);
        }

        TraceSourceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TraceSourceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TraceSourceProvisioningChanged>.Invalid(
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

        TraceSourceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TraceSourceProvisioningChanged>.Success(changed);
    }
}