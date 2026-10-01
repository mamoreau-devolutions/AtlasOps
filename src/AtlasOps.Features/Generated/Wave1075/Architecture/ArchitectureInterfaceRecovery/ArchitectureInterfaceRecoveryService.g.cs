namespace AtlasOps.Features.Architecture.ArchitectureInterfaceRecovery;

using AtlasOps.Features;

public sealed class ArchitectureInterfaceRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureInterfaceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureInterfaceRecoveryValidator validator = new();
    private readonly ArchitectureInterfaceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureInterfaceRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureInterfaceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureInterfaceRecoveryChanged>.Invalid(issues);
        }

        ArchitectureInterfaceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureInterfaceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureInterfaceRecoveryChanged>.Invalid(
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

        ArchitectureInterfaceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureInterfaceRecoveryChanged>.Success(changed);
    }
}