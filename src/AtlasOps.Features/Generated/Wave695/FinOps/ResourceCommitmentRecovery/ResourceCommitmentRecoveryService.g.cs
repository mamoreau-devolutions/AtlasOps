namespace AtlasOps.Features.FinOps.ResourceCommitmentRecovery;

using AtlasOps.Features;

public sealed class ResourceCommitmentRecoveryService(
    IAtlasOpsCapabilityRepository<ResourceCommitmentRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ResourceCommitmentRecoveryValidator validator = new();
    private readonly ResourceCommitmentRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ResourceCommitmentRecoveryChanged>> ExecuteAsync(
        UpdateResourceCommitmentRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ResourceCommitmentRecoveryChanged>.Invalid(issues);
        }

        ResourceCommitmentRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ResourceCommitmentRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ResourceCommitmentRecoveryChanged>.Invalid(
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

        ResourceCommitmentRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ResourceCommitmentRecoveryChanged>.Success(changed);
    }
}