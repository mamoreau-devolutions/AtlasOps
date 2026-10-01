namespace AtlasOps.Features.Governance.AuthorizationDecision;

using AtlasOps.Features;

public sealed class AuthorizationDecisionService(
    IAtlasOpsCapabilityRepository<AuthorizationDecisionItem> repository,
    TimeProvider timeProvider)
{
    private readonly AuthorizationDecisionValidator validator = new();
    private readonly AuthorizationDecisionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AuthorizationDecisionChanged>> ExecuteAsync(
        UpdateAuthorizationDecisionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AuthorizationDecisionChanged>.Invalid(issues);
        }

        AuthorizationDecisionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AuthorizationDecisionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AuthorizationDecisionChanged>.Invalid(
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

        AuthorizationDecisionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AuthorizationDecisionChanged>.Success(changed);
    }
}