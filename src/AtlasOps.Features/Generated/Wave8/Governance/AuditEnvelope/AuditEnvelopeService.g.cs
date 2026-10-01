namespace AtlasOps.Features.Governance.AuditEnvelope;

using AtlasOps.Features;

public sealed class AuditEnvelopeService(
    IAtlasOpsCapabilityRepository<AuditEnvelopeItem> repository,
    TimeProvider timeProvider)
{
    private readonly AuditEnvelopeValidator validator = new();
    private readonly AuditEnvelopePolicy policy = new();

    public async Task<AtlasOpsOperationResult<AuditEnvelopeChanged>> ExecuteAsync(
        UpdateAuditEnvelopeCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AuditEnvelopeChanged>.Invalid(issues);
        }

        AuditEnvelopeItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AuditEnvelopeItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AuditEnvelopeChanged>.Invalid(
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

        AuditEnvelopeChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AuditEnvelopeChanged>.Success(changed);
    }
}