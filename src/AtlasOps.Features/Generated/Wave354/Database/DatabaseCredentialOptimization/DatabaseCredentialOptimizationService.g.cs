namespace AtlasOps.Features.Database.DatabaseCredentialOptimization;

using AtlasOps.Features;

public sealed class DatabaseCredentialOptimizationService(
    IAtlasOpsCapabilityRepository<DatabaseCredentialOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseCredentialOptimizationValidator validator = new();
    private readonly DatabaseCredentialOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseCredentialOptimizationChanged>> ExecuteAsync(
        UpdateDatabaseCredentialOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseCredentialOptimizationChanged>.Invalid(issues);
        }

        DatabaseCredentialOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseCredentialOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseCredentialOptimizationChanged>.Invalid(
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

        DatabaseCredentialOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseCredentialOptimizationChanged>.Success(changed);
    }
}