namespace AtlasOps.Features.Incidents.CommunicationChannel;

using AtlasOps.Features;

public sealed class CommunicationChannelService(
    IAtlasOpsCapabilityRepository<CommunicationChannelItem> repository,
    TimeProvider timeProvider)
{
    private readonly CommunicationChannelValidator validator = new();
    private readonly CommunicationChannelPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CommunicationChannelChanged>> ExecuteAsync(
        UpdateCommunicationChannelCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CommunicationChannelChanged>.Invalid(issues);
        }

        CommunicationChannelItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CommunicationChannelItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CommunicationChannelChanged>.Invalid(
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

        CommunicationChannelChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CommunicationChannelChanged>.Success(changed);
    }
}