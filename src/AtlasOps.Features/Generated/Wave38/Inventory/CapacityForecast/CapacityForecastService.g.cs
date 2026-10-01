namespace AtlasOps.Features.Inventory.CapacityForecast;

using AtlasOps.Features;

public sealed class CapacityForecastService(
    IAtlasOpsCapabilityRepository<CapacityForecastItem> repository,
    TimeProvider timeProvider)
{
    private readonly CapacityForecastValidator validator = new();
    private readonly CapacityForecastPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CapacityForecastChanged>> ExecuteAsync(
        UpdateCapacityForecastCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CapacityForecastChanged>.Invalid(issues);
        }

        CapacityForecastItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CapacityForecastItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CapacityForecastChanged>.Invalid(
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

        CapacityForecastChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CapacityForecastChanged>.Success(changed);
    }
}