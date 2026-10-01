namespace AtlasOps.Features.FinOps.FinOpsReportRecovery;

using AtlasOps.Features;

public sealed class FinOpsReportRecoveryService(
    IAtlasOpsCapabilityRepository<FinOpsReportRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly FinOpsReportRecoveryValidator validator = new();
    private readonly FinOpsReportRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<FinOpsReportRecoveryChanged>> ExecuteAsync(
        UpdateFinOpsReportRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FinOpsReportRecoveryChanged>.Invalid(issues);
        }

        FinOpsReportRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FinOpsReportRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FinOpsReportRecoveryChanged>.Invalid(
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

        FinOpsReportRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FinOpsReportRecoveryChanged>.Success(changed);
    }
}