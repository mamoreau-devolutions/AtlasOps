namespace AtlasOps.Features.Sync.SchemaMigration;

using AtlasOps.Features;

public sealed class SchemaMigrationService(
    IAtlasOpsCapabilityRepository<SchemaMigrationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SchemaMigrationValidator validator = new();
    private readonly SchemaMigrationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SchemaMigrationChanged>> ExecuteAsync(
        UpdateSchemaMigrationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SchemaMigrationChanged>.Invalid(issues);
        }

        SchemaMigrationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SchemaMigrationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SchemaMigrationChanged>.Invalid(
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

        SchemaMigrationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SchemaMigrationChanged>.Success(changed);
    }
}