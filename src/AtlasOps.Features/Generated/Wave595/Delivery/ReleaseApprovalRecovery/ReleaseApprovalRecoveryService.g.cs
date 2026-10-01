namespace AtlasOps.Features.Delivery.ReleaseApprovalRecovery;

using AtlasOps.Features;

public sealed class ReleaseApprovalRecoveryService(
    IAtlasOpsCapabilityRepository<ReleaseApprovalRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseApprovalRecoveryValidator validator = new();
    private readonly ReleaseApprovalRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseApprovalRecoveryChanged>> ExecuteAsync(
        UpdateReleaseApprovalRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseApprovalRecoveryChanged>.Invalid(issues);
        }

        ReleaseApprovalRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseApprovalRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseApprovalRecoveryChanged>.Invalid(
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

        ReleaseApprovalRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseApprovalRecoveryChanged>.Success(changed);
    }
}