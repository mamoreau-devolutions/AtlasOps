namespace AtlasOps.Features.Delivery.BuildPipelineRecovery;

using AtlasOps.Features;

public sealed class BuildPipelineRecoveryService(
    IAtlasOpsCapabilityRepository<BuildPipelineRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildPipelineRecoveryValidator validator = new();
    private readonly BuildPipelineRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildPipelineRecoveryChanged>> ExecuteAsync(
        UpdateBuildPipelineRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildPipelineRecoveryChanged>.Invalid(issues);
        }

        BuildPipelineRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildPipelineRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildPipelineRecoveryChanged>.Invalid(
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

        BuildPipelineRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildPipelineRecoveryChanged>.Success(changed);
    }
}