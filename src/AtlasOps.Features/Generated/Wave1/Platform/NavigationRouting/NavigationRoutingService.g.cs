namespace AtlasOps.Features.Platform.NavigationRouting;

using AtlasOps.Features;

public sealed class NavigationRoutingService(
    IAtlasOpsCapabilityRepository<NavigationRoutingItem> repository,
    TimeProvider timeProvider)
{
    private readonly NavigationRoutingValidator validator = new();
    private readonly NavigationRoutingPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NavigationRoutingChanged>> ExecuteAsync(
        UpdateNavigationRoutingCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NavigationRoutingChanged>.Invalid(issues);
        }

        NavigationRoutingItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NavigationRoutingItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NavigationRoutingChanged>.Invalid(
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

        NavigationRoutingChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NavigationRoutingChanged>.Success(changed);
    }
}