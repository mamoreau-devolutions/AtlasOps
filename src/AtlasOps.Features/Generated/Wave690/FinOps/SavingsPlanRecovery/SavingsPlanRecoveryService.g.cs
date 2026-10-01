namespace AtlasOps.Features.FinOps.SavingsPlanRecovery;

using AtlasOps.Features;

public sealed class SavingsPlanRecoveryService(
    IAtlasOpsCapabilityRepository<SavingsPlanRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SavingsPlanRecoveryValidator validator = new();
    private readonly SavingsPlanRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SavingsPlanRecoveryChanged>> ExecuteAsync(
        UpdateSavingsPlanRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SavingsPlanRecoveryChanged>.Invalid(issues);
        }

        SavingsPlanRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SavingsPlanRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SavingsPlanRecoveryChanged>.Invalid(
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

        SavingsPlanRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SavingsPlanRecoveryChanged>.Success(changed);
    }
}