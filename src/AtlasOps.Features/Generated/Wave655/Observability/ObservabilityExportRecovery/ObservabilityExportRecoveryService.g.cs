namespace AtlasOps.Features.Observability.ObservabilityExportRecovery;

using AtlasOps.Features;

public sealed class ObservabilityExportRecoveryService(
    IAtlasOpsCapabilityRepository<ObservabilityExportRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityExportRecoveryValidator validator = new();
    private readonly ObservabilityExportRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityExportRecoveryChanged>> ExecuteAsync(
        UpdateObservabilityExportRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityExportRecoveryChanged>.Invalid(issues);
        }

        ObservabilityExportRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityExportRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityExportRecoveryChanged>.Invalid(
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

        ObservabilityExportRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityExportRecoveryChanged>.Success(changed);
    }
}