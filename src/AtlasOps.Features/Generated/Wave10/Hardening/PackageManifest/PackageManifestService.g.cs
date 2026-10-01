namespace AtlasOps.Features.Hardening.PackageManifest;

using AtlasOps.Features;

public sealed class PackageManifestService(
    IAtlasOpsCapabilityRepository<PackageManifestItem> repository,
    TimeProvider timeProvider)
{
    private readonly PackageManifestValidator validator = new();
    private readonly PackageManifestPolicy policy = new();

    public async Task<AtlasOpsOperationResult<PackageManifestChanged>> ExecuteAsync(
        UpdatePackageManifestCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<PackageManifestChanged>.Invalid(issues);
        }

        PackageManifestItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new PackageManifestItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<PackageManifestChanged>.Invalid(
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

        PackageManifestChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<PackageManifestChanged>.Success(changed);
    }
}