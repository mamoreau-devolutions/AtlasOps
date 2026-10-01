namespace AtlasOps.Features.Api.ApiContractMonitoring;

using AtlasOps.Features;

public sealed class ApiContractMonitoringService(
    IAtlasOpsCapabilityRepository<ApiContractMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiContractMonitoringValidator validator = new();
    private readonly ApiContractMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiContractMonitoringChanged>> ExecuteAsync(
        UpdateApiContractMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiContractMonitoringChanged>.Invalid(issues);
        }

        ApiContractMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiContractMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiContractMonitoringChanged>.Invalid(
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

        ApiContractMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiContractMonitoringChanged>.Success(changed);
    }
}