namespace AtlasOps.Features.Connections.CredentialReference;

using AtlasOps.Features;

public sealed class CredentialReferenceService(
    IAtlasOpsCapabilityRepository<CredentialReferenceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CredentialReferenceValidator validator = new();
    private readonly CredentialReferencePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CredentialReferenceChanged>> ExecuteAsync(
        UpdateCredentialReferenceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CredentialReferenceChanged>.Invalid(issues);
        }

        CredentialReferenceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CredentialReferenceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CredentialReferenceChanged>.Invalid(
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

        CredentialReferenceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CredentialReferenceChanged>.Success(changed);
    }
}