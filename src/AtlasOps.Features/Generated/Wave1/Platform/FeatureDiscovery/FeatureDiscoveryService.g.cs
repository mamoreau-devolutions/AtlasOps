namespace AtlasOps.Features.Platform.FeatureDiscovery;

using AtlasOps.Features;

public sealed class FeatureDiscoveryService(
    IAtlasOpsCapabilityRepository<FeatureDiscoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly FeatureDiscoveryValidator validator = new();
    private readonly FeatureDiscoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<FeatureDiscoveryChanged>> ExecuteAsync(
        UpdateFeatureDiscoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FeatureDiscoveryChanged>.Invalid(issues);
        }

        FeatureDiscoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FeatureDiscoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FeatureDiscoveryChanged>.Invalid(
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

        FeatureDiscoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FeatureDiscoveryChanged>.Success(changed);
    }
}