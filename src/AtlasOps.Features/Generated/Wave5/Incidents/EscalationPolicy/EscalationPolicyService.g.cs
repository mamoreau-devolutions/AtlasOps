namespace AtlasOps.Features.Incidents.EscalationPolicy;

using AtlasOps.Features;

public sealed class EscalationPolicyService(
    IAtlasOpsCapabilityRepository<EscalationPolicyItem> repository,
    TimeProvider timeProvider)
{
    private readonly EscalationPolicyValidator validator = new();
    private readonly EscalationPolicyPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EscalationPolicyChanged>> ExecuteAsync(
        UpdateEscalationPolicyCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EscalationPolicyChanged>.Invalid(issues);
        }

        EscalationPolicyItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EscalationPolicyItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EscalationPolicyChanged>.Invalid(
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

        EscalationPolicyChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EscalationPolicyChanged>.Success(changed);
    }
}