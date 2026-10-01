namespace AtlasOps.Features.Connections.SecretLease;

using AtlasOps.Features;

public sealed class SecretLeaseService(
    IAtlasOpsCapabilityRepository<SecretLeaseItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecretLeaseValidator validator = new();
    private readonly SecretLeasePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecretLeaseChanged>> ExecuteAsync(
        UpdateSecretLeaseCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecretLeaseChanged>.Invalid(issues);
        }

        SecretLeaseItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecretLeaseItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecretLeaseChanged>.Invalid(
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

        SecretLeaseChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecretLeaseChanged>.Success(changed);
    }
}