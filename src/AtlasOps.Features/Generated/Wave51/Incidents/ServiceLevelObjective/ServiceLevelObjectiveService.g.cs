namespace AtlasOps.Features.Incidents.ServiceLevelObjective;

using AtlasOps.Features;

public sealed class ServiceLevelObjectiveService(
    IAtlasOpsCapabilityRepository<ServiceLevelObjectiveItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceLevelObjectiveValidator validator = new();
    private readonly ServiceLevelObjectivePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceLevelObjectiveChanged>> ExecuteAsync(
        UpdateServiceLevelObjectiveCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceLevelObjectiveChanged>.Invalid(issues);
        }

        ServiceLevelObjectiveItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceLevelObjectiveItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceLevelObjectiveChanged>.Invalid(
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

        ServiceLevelObjectiveChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceLevelObjectiveChanged>.Success(changed);
    }
}