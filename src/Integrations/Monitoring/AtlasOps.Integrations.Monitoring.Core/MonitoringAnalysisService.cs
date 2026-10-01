namespace AtlasOps.Integrations.Monitoring.Core;

using AtlasOps.Integrations.Monitoring.Contracts;

public sealed class MonitoringAnalysisService
{
    private static readonly IReadOnlyDictionary<string, int> SeverityRanks =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["information"] = 0,
            ["warning"] = 1,
            ["error"] = 2,
            ["critical"] = 3,
        };

    public MetricWindow CreateWindow(
        IReadOnlyList<MetricSample> samples,
        DateTimeOffset start,
        DateTimeOffset end)
    {
        if (end <= start)
        {
            return new MetricWindow(start, end, 0d, 0d, 0d, 0);
        }

        double[] values = samples
            .Where(sample => sample.Timestamp >= start && sample.Timestamp < end)
            .OrderBy(static sample => sample.Timestamp)
            .Select(static sample => sample.Value)
            .ToArray();
        if (values.Length == 0)
        {
            return new MetricWindow(start, end, 0d, 0d, 0d, 0);
        }

        return new MetricWindow(
            start,
            end,
            values.Min(),
            values.Max(),
            values.Average(),
            values.Length);
    }

    public IReadOnlyList<AlertCorrelationGroup> Correlate(
        IReadOnlyList<AlertSignal> alerts,
        TimeSpan correlationWindow)
    {
        List<AlertCorrelationGroup> groups = [];
        foreach (IGrouping<string, AlertSignal> resourceGroup in alerts
                     .OrderBy(static alert => alert.StartedAt)
                     .GroupBy(static alert => alert.ResourceId, StringComparer.OrdinalIgnoreCase))
        {
            List<AlertSignal> current = [];
            DateTimeOffset? windowStart = null;
            foreach (AlertSignal alert in resourceGroup)
            {
                if (windowStart is not null &&
                    alert.StartedAt - windowStart.Value > correlationWindow)
                {
                    groups.Add(CreateGroup(resourceGroup.Key, current));
                    current = [];
                    windowStart = null;
                }

                windowStart ??= alert.StartedAt;
                current.Add(alert);
            }

            if (current.Count > 0)
            {
                groups.Add(CreateGroup(resourceGroup.Key, current));
            }
        }

        return groups
            .OrderByDescending(static group => group.StartedAt)
            .ThenBy(static group => group.ResourceId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static AlertCorrelationGroup CreateGroup(
        string resourceId,
        IReadOnlyList<AlertSignal> alerts)
    {
        string highestSeverity = alerts
            .OrderByDescending(alert => SeverityRanks.GetValueOrDefault(alert.Severity, -1))
            .Select(static alert => alert.Severity)
            .First();
        DateTimeOffset? resolvedAt = alerts.All(static alert => alert.ResolvedAt is not null)
            ? alerts.Max(static alert => alert.ResolvedAt)
            : null;
        string correlationKey = $"{resourceId}:{alerts.Min(static alert => alert.StartedAt).ToUnixTimeSeconds()}";
        return new AlertCorrelationGroup(
            correlationKey,
            resourceId,
            highestSeverity,
            alerts.Min(static alert => alert.StartedAt),
            resolvedAt,
            alerts.ToArray());
    }
}
