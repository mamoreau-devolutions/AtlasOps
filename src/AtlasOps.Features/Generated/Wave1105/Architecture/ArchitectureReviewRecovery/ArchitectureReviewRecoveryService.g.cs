namespace AtlasOps.Features.Architecture.ArchitectureReviewRecovery;

using AtlasOps.Features;

public sealed class ArchitectureReviewRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureReviewRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureReviewRecoveryValidator validator = new();
    private readonly ArchitectureReviewRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureReviewRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureReviewRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureReviewRecoveryChanged>.Invalid(issues);
        }

        ArchitectureReviewRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureReviewRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureReviewRecoveryChanged>.Invalid(
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

        ArchitectureReviewRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureReviewRecoveryChanged>.Success(changed);
    }
}