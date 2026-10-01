namespace AtlasOps.Features.Connections.TunnelDefinition;

using AtlasOps.Features;

public sealed class TunnelDefinitionService(
    IAtlasOpsCapabilityRepository<TunnelDefinitionItem> repository,
    TimeProvider timeProvider)
{
    private readonly TunnelDefinitionValidator validator = new();
    private readonly TunnelDefinitionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TunnelDefinitionChanged>> ExecuteAsync(
        UpdateTunnelDefinitionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TunnelDefinitionChanged>.Invalid(issues);
        }

        TunnelDefinitionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TunnelDefinitionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TunnelDefinitionChanged>.Invalid(
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

        TunnelDefinitionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TunnelDefinitionChanged>.Success(changed);
    }
}