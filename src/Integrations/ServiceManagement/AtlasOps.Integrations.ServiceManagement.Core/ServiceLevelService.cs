namespace AtlasOps.Integrations.ServiceManagement.Core;

using AtlasOps.Integrations.ServiceManagement.Contracts;

public sealed class ServiceLevelService
{
    public ServiceLevelEvaluation Evaluate(
        ServiceTicket ticket,
        ServiceLevelPolicy policy,
        DateTimeOffset now)
    {
        List<string> diagnostics = [];
        if (!string.Equals(ticket.Priority, policy.Priority, StringComparison.OrdinalIgnoreCase))
        {
            diagnostics.Add("The service-level policy does not match the ticket priority.");
        }

        DateTimeOffset effectiveEnd = ticket.ResolvedAt ?? now;
        TimeSpan activePause = ticket.PausedAt is not null
            ? effectiveEnd - ticket.PausedAt.Value
            : TimeSpan.Zero;
        TimeSpan activeAge = effectiveEnd - ticket.CreatedAt -
                             ticket.AccumulatedPausedTime -
                             activePause;
        if (activeAge < TimeSpan.Zero)
        {
            diagnostics.Add("Ticket chronology is invalid.");
            activeAge = TimeSpan.Zero;
        }

        TimeSpan remaining = policy.ResolutionTarget - activeAge;
        int escalationLevel = policy.EscalationThresholds.Count(
            threshold => activeAge >= threshold);
        return new ServiceLevelEvaluation(
            activeAge,
            remaining,
            escalationLevel,
            remaining <= TimeSpan.Zero,
            diagnostics);
    }

    public ServiceTicket ApplyTransition(
        ServiceTicket ticket,
        TicketTransition transition,
        DateTimeOffset now,
        string nextRevision)
    {
        if (!transition.FromStates.Contains(ticket.State))
        {
            return ticket;
        }

        TimeSpan accumulatedPause = ticket.AccumulatedPausedTime;
        DateTimeOffset? pausedAt = ticket.PausedAt;
        if (ticket.PausedAt is not null && !transition.PausesServiceLevel)
        {
            accumulatedPause += now - ticket.PausedAt.Value;
            pausedAt = null;
        }
        else if (ticket.PausedAt is null && transition.PausesServiceLevel)
        {
            pausedAt = now;
        }

        return ticket with
        {
            State = transition.TargetState,
            UpdatedAt = now,
            ResolvedAt = transition.ResolvesTicket ? now : ticket.ResolvedAt,
            AccumulatedPausedTime = accumulatedPause,
            PausedAt = pausedAt,
            Revision = nextRevision,
        };
    }

    public IReadOnlyList<DateTimeOffset> CreateEscalationSchedule(
        ServiceTicket ticket,
        ServiceLevelPolicy policy)
    {
        DateTimeOffset baseTime = ticket.CreatedAt + ticket.AccumulatedPausedTime;
        return policy.EscalationThresholds
            .Order()
            .Select(baseTime.Add)
            .ToArray();
    }
}
