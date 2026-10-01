namespace AtlasOps.Features.Mobile.MobileTelemetryRecovery;

using AtlasOps.Features;

public sealed class MobileTelemetryRecoveryService(
    IAtlasOpsCapabilityRepository<MobileTelemetryRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileTelemetryRecoveryValidator validator = new();
    private readonly MobileTelemetryRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileTelemetryRecoveryChanged>> ExecuteAsync(
        UpdateMobileTelemetryRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileTelemetryRecoveryChanged>.Invalid(issues);
        }

        MobileTelemetryRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileTelemetryRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileTelemetryRecoveryChanged>.Invalid(
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

        MobileTelemetryRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileTelemetryRecoveryChanged>.Success(changed);
    }
}