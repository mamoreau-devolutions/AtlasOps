namespace AtlasOps.Features.Governance.PermissionGrant;

using AtlasOps.Features;

public sealed class PermissionGrantService(
    IAtlasOpsCapabilityRepository<PermissionGrantItem> repository,
    TimeProvider timeProvider)
{
    private readonly PermissionGrantValidator validator = new();
    private readonly PermissionGrantPolicy policy = new();

    public async Task<AtlasOpsOperationResult<PermissionGrantChanged>> ExecuteAsync(
        UpdatePermissionGrantCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<PermissionGrantChanged>.Invalid(issues);
        }

        PermissionGrantItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new PermissionGrantItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<PermissionGrantChanged>.Invalid(
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

        PermissionGrantChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<PermissionGrantChanged>.Success(changed);
    }
}