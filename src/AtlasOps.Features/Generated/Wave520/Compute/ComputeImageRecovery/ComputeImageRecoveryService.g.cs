namespace AtlasOps.Features.Compute.ComputeImageRecovery;

using AtlasOps.Features;

public sealed class ComputeImageRecoveryService(
    IAtlasOpsCapabilityRepository<ComputeImageRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeImageRecoveryValidator validator = new();
    private readonly ComputeImageRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeImageRecoveryChanged>> ExecuteAsync(
        UpdateComputeImageRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeImageRecoveryChanged>.Invalid(issues);
        }

        ComputeImageRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeImageRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeImageRecoveryChanged>.Invalid(
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

        ComputeImageRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeImageRecoveryChanged>.Success(changed);
    }
}