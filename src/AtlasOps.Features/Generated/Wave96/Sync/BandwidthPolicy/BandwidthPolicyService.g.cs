namespace AtlasOps.Features.Sync.BandwidthPolicy;

using AtlasOps.Features;

public sealed class BandwidthPolicyService(
    IAtlasOpsCapabilityRepository<BandwidthPolicyItem> repository,
    TimeProvider timeProvider)
{
    private readonly BandwidthPolicyValidator validator = new();
    private readonly BandwidthPolicyPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BandwidthPolicyChanged>> ExecuteAsync(
        UpdateBandwidthPolicyCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BandwidthPolicyChanged>.Invalid(issues);
        }

        BandwidthPolicyItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BandwidthPolicyItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BandwidthPolicyChanged>.Invalid(
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

        BandwidthPolicyChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BandwidthPolicyChanged>.Success(changed);
    }
}