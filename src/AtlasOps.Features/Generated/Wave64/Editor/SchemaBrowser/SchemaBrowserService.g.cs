namespace AtlasOps.Features.Editor.SchemaBrowser;

using AtlasOps.Features;

public sealed class SchemaBrowserService(
    IAtlasOpsCapabilityRepository<SchemaBrowserItem> repository,
    TimeProvider timeProvider)
{
    private readonly SchemaBrowserValidator validator = new();
    private readonly SchemaBrowserPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SchemaBrowserChanged>> ExecuteAsync(
        UpdateSchemaBrowserCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SchemaBrowserChanged>.Invalid(issues);
        }

        SchemaBrowserItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SchemaBrowserItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SchemaBrowserChanged>.Invalid(
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

        SchemaBrowserChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SchemaBrowserChanged>.Success(changed);
    }
}