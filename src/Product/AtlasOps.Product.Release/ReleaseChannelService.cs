namespace AtlasOps.Product.Release;

public enum ReleaseChannel
{
    Stable,
    Preview,
    Canary,
}

public sealed record ReleaseArtifact(
    Version Version,
    ReleaseChannel Channel,
    Uri DownloadUri,
    string Sha256,
    DateTimeOffset PublishedAt,
    Version MinimumSupportedVersion,
    bool Mandatory);

public sealed record UpdatePolicy(
    ReleaseChannel Channel,
    bool AllowDowngrade,
    bool AllowPrerelease,
    TimeSpan MinimumAge);

public sealed record UpdateDecision(
    bool Update,
    ReleaseArtifact? Artifact,
    IReadOnlyList<string> Diagnostics);

public sealed class ReleaseChannelService
{
    public UpdateDecision Select(
        Version currentVersion,
        UpdatePolicy policy,
        IReadOnlyList<ReleaseArtifact> artifacts,
        DateTimeOffset now)
    {
        List<string> diagnostics = [];
        ReleaseArtifact? selected = artifacts
            .Where(artifact => IsChannelAllowed(policy.Channel, artifact.Channel))
            .Where(artifact => policy.AllowPrerelease || artifact.Channel == ReleaseChannel.Stable)
            .Where(artifact => now - artifact.PublishedAt >= policy.MinimumAge || artifact.Mandatory)
            .OrderByDescending(static artifact => artifact.Version)
            .FirstOrDefault();

        if (selected is null)
        {
            diagnostics.Add("No release satisfies the configured update policy.");
            return new UpdateDecision(false, null, diagnostics);
        }

        if (selected.Version < currentVersion && !policy.AllowDowngrade)
        {
            diagnostics.Add("Selected release is older than the installed version.");
            return new UpdateDecision(false, selected, diagnostics);
        }

        if (selected.Version == currentVersion)
        {
            diagnostics.Add("The installed version is current.");
            return new UpdateDecision(false, selected, diagnostics);
        }

        if (currentVersion < selected.MinimumSupportedVersion)
        {
            diagnostics.Add("Installed version requires a mandatory upgrade path.");
        }

        return new UpdateDecision(true, selected, diagnostics);
    }

    private static bool IsChannelAllowed(ReleaseChannel configured, ReleaseChannel candidate)
    {
        return configured switch
        {
            ReleaseChannel.Stable => candidate == ReleaseChannel.Stable,
            ReleaseChannel.Preview => candidate is ReleaseChannel.Stable or ReleaseChannel.Preview,
            ReleaseChannel.Canary => true,
            _ => false,
        };
    }
}
