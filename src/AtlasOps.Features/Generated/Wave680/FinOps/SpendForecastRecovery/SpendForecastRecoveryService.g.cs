namespace AtlasOps.Features.FinOps.SpendForecastRecovery;

using AtlasOps.Features;

public sealed class SpendForecastRecoveryService(
    IAtlasOpsCapabilityRepository<SpendForecastRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SpendForecastRecoveryValidator validator = new();
    private readonly SpendForecastRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SpendForecastRecoveryChanged>> ExecuteAsync(
        UpdateSpendForecastRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SpendForecastRecoveryChanged>.Invalid(issues);
        }

        SpendForecastRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SpendForecastRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SpendForecastRecoveryChanged>.Invalid(
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

        SpendForecastRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SpendForecastRecoveryChanged>.Success(changed);
    }
}