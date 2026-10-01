namespace AtlasOps.Features.Connections.ProxyProfile;

using AtlasOps.Features;

public sealed class ProxyProfileService(
    IAtlasOpsCapabilityRepository<ProxyProfileItem> repository,
    TimeProvider timeProvider)
{
    private readonly ProxyProfileValidator validator = new();
    private readonly ProxyProfilePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ProxyProfileChanged>> ExecuteAsync(
        UpdateProxyProfileCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ProxyProfileChanged>.Invalid(issues);
        }

        ProxyProfileItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ProxyProfileItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ProxyProfileChanged>.Invalid(
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

        ProxyProfileChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ProxyProfileChanged>.Success(changed);
    }
}