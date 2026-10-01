namespace AtlasOps.Features.Platform.SettingsManagement;

using AtlasOps.Features;

public sealed class SettingsManagementService(
    IAtlasOpsCapabilityRepository<SettingsManagementItem> repository,
    TimeProvider timeProvider)
{
    private readonly SettingsManagementValidator validator = new();
    private readonly SettingsManagementPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SettingsManagementChanged>> ExecuteAsync(
        UpdateSettingsManagementCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SettingsManagementChanged>.Invalid(issues);
        }

        SettingsManagementItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SettingsManagementItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SettingsManagementChanged>.Invalid(
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

        SettingsManagementChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SettingsManagementChanged>.Success(changed);
    }
}