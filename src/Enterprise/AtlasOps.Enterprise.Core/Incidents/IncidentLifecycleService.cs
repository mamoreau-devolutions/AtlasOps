namespace AtlasOps.Enterprise.Core.Incidents;

using AtlasOps.Enterprise.Contracts.Incidents;

public sealed class IncidentLifecycleService
{
    private static readonly IReadOnlyDictionary<IncidentState, IReadOnlySet<IncidentState>> AllowedTransitions =
        new Dictionary<IncidentState, IReadOnlySet<IncidentState>>
        {
            [IncidentState.New] = new HashSet<IncidentState> { IncidentState.Triaged, IncidentState.Cancelled },
            [IncidentState.Triaged] = new HashSet<IncidentState> { IncidentState.Acknowledged, IncidentState.Cancelled },
            [IncidentState.Acknowledged] = new HashSet<IncidentState> { IncidentState.Investigating, IncidentState.Cancelled },
            [IncidentState.Investigating] = new HashSet<IncidentState> { IncidentState.Mitigated, IncidentState.Resolved, IncidentState.Cancelled },
            [IncidentState.Mitigated] = new HashSet<IncidentState> { IncidentState.Investigating, IncidentState.Resolved },
            [IncidentState.Resolved] = new HashSet<IncidentState> { IncidentState.Investigating, IncidentState.Closed },
            [IncidentState.Closed] = new HashSet<IncidentState>(),
            [IncidentState.Cancelled] = new HashSet<IncidentState>(),
        };

    public IncidentTransitionResult Transition(
        Incident incident,
        IncidentState targetState,
        string actor,
        DateTimeOffset occurredAt,
        string? reason = null)
    {
        if (!AllowedTransitions[incident.State].Contains(targetState))
        {
            return new(false, incident, null, $"Transition from {incident.State} to {targetState} is not allowed.");
        }

        DateTimeOffset? acknowledgedAt = incident.AcknowledgedAt;
        DateTimeOffset? resolvedAt = incident.ResolvedAt;
        if (targetState == IncidentState.Acknowledged && acknowledgedAt is null)
        {
            acknowledgedAt = occurredAt;
        }

        if (targetState == IncidentState.Resolved)
        {
            resolvedAt = occurredAt;
        }
        else if (targetState == IncidentState.Investigating && incident.State == IncidentState.Resolved)
        {
            resolvedAt = null;
        }

        Incident updated = incident with
        {
            State = targetState,
            UpdatedAt = occurredAt,
            AcknowledgedAt = acknowledgedAt,
            ResolvedAt = resolvedAt,
        };
        string message = string.IsNullOrWhiteSpace(reason)
            ? $"State changed from {incident.State} to {targetState}."
            : $"State changed from {incident.State} to {targetState}: {reason}";
        IncidentTimelineEntry timelineEntry = new(
            Guid.NewGuid().ToString("N"),
            incident.Id,
            targetState == IncidentState.Resolved ? IncidentTimelineKind.Resolved : IncidentTimelineKind.StateChanged,
            occurredAt,
            actor,
            message,
            new Dictionary<string, string>
            {
                ["previousState"] = incident.State.ToString(),
                ["newState"] = targetState.ToString(),
            });
        return new(true, updated, timelineEntry, message);
    }

    public bool CanTransition(IncidentState currentState, IncidentState targetState)
    {
        return AllowedTransitions[currentState].Contains(targetState);
    }

    public IReadOnlyList<IncidentState> GetAvailableTransitions(IncidentState currentState)
    {
        return AllowedTransitions[currentState].Order().ToArray();
    }
}
