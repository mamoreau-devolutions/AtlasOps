namespace AtlasOps.Features.Delivery.ReleasePipelineRecovery;

using AtlasOps.Features;

public sealed class ReleasePipelineRecoveryService(
    IAtlasOpsCapabilityRepository<ReleasePipelineRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleasePipelineRecoveryValidator validator = new();
    private readonly ReleasePipelineRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleasePipelineRecoveryChanged>> ExecuteAsync(
        UpdateReleasePipelineRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleasePipelineRecoveryChanged>.Invalid(issues);
        }

        ReleasePipelineRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleasePipelineRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleasePipelineRecoveryChanged>.Invalid(
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

        ReleasePipelineRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleasePipelineRecoveryChanged>.Success(changed);
    }
}