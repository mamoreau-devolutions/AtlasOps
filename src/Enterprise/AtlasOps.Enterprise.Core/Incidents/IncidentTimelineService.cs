namespace AtlasOps.Enterprise.Core.Incidents;

using AtlasOps.Enterprise.Contracts.Incidents;

public sealed class IncidentTimelineService
{
    public IReadOnlyList<IncidentTimelineEntry> Append(
        IReadOnlyList<IncidentTimelineEntry> timeline,
        IncidentTimelineEntry entry)
    {
        if (timeline.Any(item => string.Equals(item.Id, entry.Id, StringComparison.OrdinalIgnoreCase)))
        {
            return timeline;
        }

        return timeline
            .Append(entry)
            .OrderBy(static item => item.OccurredAt)
            .ThenBy(static item => item.Id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public IReadOnlyList<IncidentTimelineEntry> CreateSummary(
        Incident incident,
        IReadOnlyList<IncidentTimelineEntry> timeline)
    {
        return timeline
            .Where(entry => string.Equals(entry.IncidentId, incident.Id, StringComparison.OrdinalIgnoreCase))
            .OrderBy(static entry => entry.OccurredAt)
            .ThenBy(static entry => entry.Id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
