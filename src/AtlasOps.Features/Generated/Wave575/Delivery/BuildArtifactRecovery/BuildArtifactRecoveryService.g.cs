namespace AtlasOps.Features.Delivery.BuildArtifactRecovery;

using AtlasOps.Features;

public sealed class BuildArtifactRecoveryService(
    IAtlasOpsCapabilityRepository<BuildArtifactRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildArtifactRecoveryValidator validator = new();
    private readonly BuildArtifactRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildArtifactRecoveryChanged>> ExecuteAsync(
        UpdateBuildArtifactRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildArtifactRecoveryChanged>.Invalid(issues);
        }

        BuildArtifactRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildArtifactRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildArtifactRecoveryChanged>.Invalid(
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

        BuildArtifactRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildArtifactRecoveryChanged>.Success(changed);
    }
}