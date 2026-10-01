namespace AtlasOps.Features.Observability.TraceSpanProvisioning;

using AtlasOps.Features;

public sealed class TraceSpanProvisioningService(
    IAtlasOpsCapabilityRepository<TraceSpanProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly TraceSpanProvisioningValidator validator = new();
    private readonly TraceSpanProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TraceSpanProvisioningChanged>> ExecuteAsync(
        UpdateTraceSpanProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TraceSpanProvisioningChanged>.Invalid(issues);
        }

        TraceSpanProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TraceSpanProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TraceSpanProvisioningChanged>.Invalid(
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

        TraceSpanProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TraceSpanProvisioningChanged>.Success(changed);
    }
}