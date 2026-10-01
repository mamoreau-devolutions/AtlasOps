namespace AtlasOps.Features.Hardening.FeatureHealth;

using AtlasOps.Features;

public sealed class FeatureHealthService(
    IAtlasOpsCapabilityRepository<FeatureHealthItem> repository,
    TimeProvider timeProvider)
{
    private readonly FeatureHealthValidator validator = new();
    private readonly FeatureHealthPolicy policy = new();

    public async Task<AtlasOpsOperationResult<FeatureHealthChanged>> ExecuteAsync(
        UpdateFeatureHealthCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FeatureHealthChanged>.Invalid(issues);
        }

        FeatureHealthItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FeatureHealthItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FeatureHealthChanged>.Invalid(
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

        FeatureHealthChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FeatureHealthChanged>.Success(changed);
    }
}