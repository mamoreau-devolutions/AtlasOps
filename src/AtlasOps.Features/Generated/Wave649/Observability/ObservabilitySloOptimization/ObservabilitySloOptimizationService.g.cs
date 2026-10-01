namespace AtlasOps.Features.Observability.ObservabilitySloOptimization;

using AtlasOps.Features;

public sealed class ObservabilitySloOptimizationService(
    IAtlasOpsCapabilityRepository<ObservabilitySloOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilitySloOptimizationValidator validator = new();
    private readonly ObservabilitySloOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilitySloOptimizationChanged>> ExecuteAsync(
        UpdateObservabilitySloOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilitySloOptimizationChanged>.Invalid(issues);
        }

        ObservabilitySloOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilitySloOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilitySloOptimizationChanged>.Invalid(
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

        ObservabilitySloOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilitySloOptimizationChanged>.Success(changed);
    }
}