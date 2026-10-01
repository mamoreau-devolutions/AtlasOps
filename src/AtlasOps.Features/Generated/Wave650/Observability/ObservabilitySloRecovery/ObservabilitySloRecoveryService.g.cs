namespace AtlasOps.Features.Observability.ObservabilitySloRecovery;

using AtlasOps.Features;

public sealed class ObservabilitySloRecoveryService(
    IAtlasOpsCapabilityRepository<ObservabilitySloRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilitySloRecoveryValidator validator = new();
    private readonly ObservabilitySloRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilitySloRecoveryChanged>> ExecuteAsync(
        UpdateObservabilitySloRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilitySloRecoveryChanged>.Invalid(issues);
        }

        ObservabilitySloRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilitySloRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilitySloRecoveryChanged>.Invalid(
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

        ObservabilitySloRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilitySloRecoveryChanged>.Success(changed);
    }
}