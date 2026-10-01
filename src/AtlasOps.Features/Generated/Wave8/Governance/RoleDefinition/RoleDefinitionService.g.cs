namespace AtlasOps.Features.Governance.RoleDefinition;

using AtlasOps.Features;

public sealed class RoleDefinitionService(
    IAtlasOpsCapabilityRepository<RoleDefinitionItem> repository,
    TimeProvider timeProvider)
{
    private readonly RoleDefinitionValidator validator = new();
    private readonly RoleDefinitionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RoleDefinitionChanged>> ExecuteAsync(
        UpdateRoleDefinitionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RoleDefinitionChanged>.Invalid(issues);
        }

        RoleDefinitionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RoleDefinitionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RoleDefinitionChanged>.Invalid(
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

        RoleDefinitionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RoleDefinitionChanged>.Success(changed);
    }
}