namespace AtlasOps.Features.Cloud.GcpProjectRecovery;

using AtlasOps.Features;

public sealed class GcpProjectRecoveryService(
    IAtlasOpsCapabilityRepository<GcpProjectRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly GcpProjectRecoveryValidator validator = new();
    private readonly GcpProjectRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<GcpProjectRecoveryChanged>> ExecuteAsync(
        UpdateGcpProjectRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<GcpProjectRecoveryChanged>.Invalid(issues);
        }

        GcpProjectRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new GcpProjectRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<GcpProjectRecoveryChanged>.Invalid(
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

        GcpProjectRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<GcpProjectRecoveryChanged>.Success(changed);
    }
}