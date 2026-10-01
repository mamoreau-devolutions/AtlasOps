namespace AtlasOps.Features.Platform.ModuleIsolation;

using AtlasOps.Features;

public sealed class ModuleIsolationService(
    IAtlasOpsCapabilityRepository<ModuleIsolationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ModuleIsolationValidator validator = new();
    private readonly ModuleIsolationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ModuleIsolationChanged>> ExecuteAsync(
        UpdateModuleIsolationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ModuleIsolationChanged>.Invalid(issues);
        }

        ModuleIsolationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ModuleIsolationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ModuleIsolationChanged>.Invalid(
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

        ModuleIsolationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ModuleIsolationChanged>.Success(changed);
    }
}