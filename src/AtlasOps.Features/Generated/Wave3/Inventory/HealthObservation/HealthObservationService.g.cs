namespace AtlasOps.Features.Inventory.HealthObservation;

using AtlasOps.Features;

public sealed class HealthObservationService(
    IAtlasOpsCapabilityRepository<HealthObservationItem> repository,
    TimeProvider timeProvider)
{
    private readonly HealthObservationValidator validator = new();
    private readonly HealthObservationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<HealthObservationChanged>> ExecuteAsync(
        UpdateHealthObservationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<HealthObservationChanged>.Invalid(issues);
        }

        HealthObservationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new HealthObservationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<HealthObservationChanged>.Invalid(
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

        HealthObservationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<HealthObservationChanged>.Success(changed);
    }
}