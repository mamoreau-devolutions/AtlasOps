namespace AtlasOps.Features.FinOps.SpendForecastGovernance;

using AtlasOps.Features;

public sealed class SpendForecastGovernanceService(
    IAtlasOpsCapabilityRepository<SpendForecastGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SpendForecastGovernanceValidator validator = new();
    private readonly SpendForecastGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SpendForecastGovernanceChanged>> ExecuteAsync(
        UpdateSpendForecastGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SpendForecastGovernanceChanged>.Invalid(issues);
        }

        SpendForecastGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SpendForecastGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SpendForecastGovernanceChanged>.Invalid(
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

        SpendForecastGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SpendForecastGovernanceChanged>.Success(changed);
    }
}