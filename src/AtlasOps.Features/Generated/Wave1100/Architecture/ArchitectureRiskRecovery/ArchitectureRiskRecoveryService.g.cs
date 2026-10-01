namespace AtlasOps.Features.Architecture.ArchitectureRiskRecovery;

using AtlasOps.Features;

public sealed class ArchitectureRiskRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureRiskRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRiskRecoveryValidator validator = new();
    private readonly ArchitectureRiskRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRiskRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureRiskRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRiskRecoveryChanged>.Invalid(issues);
        }

        ArchitectureRiskRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRiskRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRiskRecoveryChanged>.Invalid(
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

        ArchitectureRiskRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRiskRecoveryChanged>.Success(changed);
    }
}