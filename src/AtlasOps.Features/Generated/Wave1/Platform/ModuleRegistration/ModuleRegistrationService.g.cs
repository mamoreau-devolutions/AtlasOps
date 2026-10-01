namespace AtlasOps.Features.Platform.ModuleRegistration;

using AtlasOps.Features;

public sealed class ModuleRegistrationService(
    IAtlasOpsCapabilityRepository<ModuleRegistrationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ModuleRegistrationValidator validator = new();
    private readonly ModuleRegistrationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ModuleRegistrationChanged>> ExecuteAsync(
        UpdateModuleRegistrationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ModuleRegistrationChanged>.Invalid(issues);
        }

        ModuleRegistrationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ModuleRegistrationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ModuleRegistrationChanged>.Invalid(
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

        ModuleRegistrationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ModuleRegistrationChanged>.Success(changed);
    }
}