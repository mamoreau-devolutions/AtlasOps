namespace AtlasOps.Features.Governance.AccessRequest;

using AtlasOps.Features;

public sealed class AccessRequestService(
    IAtlasOpsCapabilityRepository<AccessRequestItem> repository,
    TimeProvider timeProvider)
{
    private readonly AccessRequestValidator validator = new();
    private readonly AccessRequestPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AccessRequestChanged>> ExecuteAsync(
        UpdateAccessRequestCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AccessRequestChanged>.Invalid(issues);
        }

        AccessRequestItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AccessRequestItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AccessRequestChanged>.Invalid(
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

        AccessRequestChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AccessRequestChanged>.Success(changed);
    }
}