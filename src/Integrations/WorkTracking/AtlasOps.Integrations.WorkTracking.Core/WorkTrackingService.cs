namespace AtlasOps.Integrations.WorkTracking.Core;

using AtlasOps.Integrations.WorkTracking.Contracts;

public sealed class WorkTrackingService
{
    public WorkTransitionResult ApplyTransition(
        WorkItemReference item,
        WorkTransition transition,
        IReadOnlyDictionary<string, string> values,
        string expectedRevision)
    {
        List<string> diagnostics = [];

        if (!string.Equals(item.Revision, expectedRevision, StringComparison.Ordinal))
        {
            diagnostics.Add("The work item revision is stale.");
        }

        if (!transition.FromStates.Contains(item.State))
        {
            diagnostics.Add(
                $"Transition '{transition.Name}' cannot start from state '{item.State}'.");
        }

        foreach (string field in transition.RequiredFields.Order(StringComparer.OrdinalIgnoreCase))
        {
            if (!values.TryGetValue(field, out string? value) || string.IsNullOrWhiteSpace(value))
            {
                diagnostics.Add($"Required field '{field}' is missing.");
            }
        }

        if (diagnostics.Count > 0)
        {
            return new WorkTransitionResult(false, item, diagnostics);
        }

        string nextRevision = IncrementRevision(item.Revision);
        WorkItemReference updated = item with
        {
            State = transition.TargetState,
            Revision = nextRevision,
        };
        return new WorkTransitionResult(true, updated, []);
    }

    public WorkSynchronizationDelta CreateDelta(
        IReadOnlyList<WorkItemReference> previous,
        IReadOnlyList<WorkItemReference> current,
        string watermark)
    {
        Dictionary<string, WorkItemReference> previousByKey = previous.ToDictionary(
            static item => item.Key,
            StringComparer.OrdinalIgnoreCase);
        HashSet<string> currentKeys = current
            .Select(static item => item.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        WorkItemReference[] changed = current
            .Where(item =>
                !previousByKey.TryGetValue(item.Key, out WorkItemReference? old) ||
                !string.Equals(old.Revision, item.Revision, StringComparison.Ordinal))
            .OrderBy(static item => item.Key, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        string[] deleted = previous
            .Where(item => !currentKeys.Contains(item.Key))
            .Select(static item => item.Key)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new WorkSynchronizationDelta(changed, deleted, watermark);
    }

    private static string IncrementRevision(string revision)
    {
        return long.TryParse(revision, out long number)
            ? checked(number + 1).ToString(System.Globalization.CultureInfo.InvariantCulture)
            : $"{revision}:1";
    }
}
