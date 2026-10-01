namespace AtlasOps.Features.ServiceManagement.ServiceScorecardRecovery;

using AtlasOps.Features;

public sealed class ServiceScorecardRecoveryService(
    IAtlasOpsCapabilityRepository<ServiceScorecardRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceScorecardRecoveryValidator validator = new();
    private readonly ServiceScorecardRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceScorecardRecoveryChanged>> ExecuteAsync(
        UpdateServiceScorecardRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceScorecardRecoveryChanged>.Invalid(issues);
        }

        ServiceScorecardRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceScorecardRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceScorecardRecoveryChanged>.Invalid(
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

        ServiceScorecardRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceScorecardRecoveryChanged>.Success(changed);
    }
}