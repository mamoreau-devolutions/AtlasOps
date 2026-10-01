namespace AtlasOps.Features.Compute.ComputeConsoleRecovery;

using AtlasOps.Features;

public sealed class ComputeConsoleRecoveryService(
    IAtlasOpsCapabilityRepository<ComputeConsoleRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeConsoleRecoveryValidator validator = new();
    private readonly ComputeConsoleRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeConsoleRecoveryChanged>> ExecuteAsync(
        UpdateComputeConsoleRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeConsoleRecoveryChanged>.Invalid(issues);
        }

        ComputeConsoleRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeConsoleRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeConsoleRecoveryChanged>.Invalid(
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

        ComputeConsoleRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeConsoleRecoveryChanged>.Success(changed);
    }
}