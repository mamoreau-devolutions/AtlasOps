namespace AtlasOps.Features.Edge.EdgeTelemetryRecovery;

using AtlasOps.Features;

public sealed class EdgeTelemetryRecoveryService(
    IAtlasOpsCapabilityRepository<EdgeTelemetryRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeTelemetryRecoveryValidator validator = new();
    private readonly EdgeTelemetryRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeTelemetryRecoveryChanged>> ExecuteAsync(
        UpdateEdgeTelemetryRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeTelemetryRecoveryChanged>.Invalid(issues);
        }

        EdgeTelemetryRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeTelemetryRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeTelemetryRecoveryChanged>.Invalid(
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

        EdgeTelemetryRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeTelemetryRecoveryChanged>.Success(changed);
    }
}