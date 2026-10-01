namespace AtlasOps.Enterprise.Core.Incidents;

using AtlasOps.Enterprise.Contracts.Incidents;

public sealed class IncidentSlaService
{
    private readonly IReadOnlyDictionary<IncidentSeverity, ServiceLevelTarget> targets;

    public IncidentSlaService(IReadOnlyList<ServiceLevelTarget> targets)
    {
        this.targets = targets.ToDictionary(static target => target.Severity);
    }

    public IncidentSlaStatus Calculate(Incident incident, DateTimeOffset now)
    {
        ServiceLevelTarget target = this.targets.TryGetValue(incident.Severity, out ServiceLevelTarget? configured)
            ? configured
            : CreateDefault(incident.Severity);
        DateTimeOffset acknowledgementDeadline = incident.CreatedAt + target.AcknowledgementTarget;
        DateTimeOffset resolutionDeadline = incident.CreatedAt + target.ResolutionTarget;
        DateTimeOffset acknowledgementReference = incident.AcknowledgedAt ?? now;
        DateTimeOffset resolutionReference = incident.ResolvedAt ?? now;
        TimeSpan acknowledgementRemaining = acknowledgementDeadline - acknowledgementReference;
        TimeSpan resolutionRemaining = resolutionDeadline - resolutionReference;
        double elapsed = Math.Max(0d, (resolutionReference - incident.CreatedAt).TotalSeconds);
        double budget = Math.Max(1d, target.ResolutionTarget.TotalSeconds);
        double consumed = Math.Clamp(elapsed / budget * 100d, 0d, 1000d);

        return new(
            acknowledgementDeadline,
            resolutionDeadline,
            acknowledgementRemaining,
            resolutionRemaining,
            incident.AcknowledgedAt is null && acknowledgementRemaining < TimeSpan.Zero,
            incident.ResolvedAt is null && resolutionRemaining < TimeSpan.Zero,
            consumed);
    }

    public ServiceLevelTarget GetTarget(IncidentSeverity severity)
    {
        return this.targets.TryGetValue(severity, out ServiceLevelTarget? target) ? target : CreateDefault(severity);
    }

    public static IReadOnlyList<ServiceLevelTarget> CreateDefaultTargets()
    {
        return
        [
            CreateDefault(IncidentSeverity.Sev1),
            CreateDefault(IncidentSeverity.Sev2),
            CreateDefault(IncidentSeverity.Sev3),
            CreateDefault(IncidentSeverity.Sev4),
        ];
    }

    private static ServiceLevelTarget CreateDefault(IncidentSeverity severity)
    {
        return severity switch
        {
            IncidentSeverity.Sev1 => new(severity, TimeSpan.FromMinutes(5), TimeSpan.FromHours(1), TimeSpan.FromMinutes(15)),
            IncidentSeverity.Sev2 => new(severity, TimeSpan.FromMinutes(15), TimeSpan.FromHours(4), TimeSpan.FromMinutes(30)),
            IncidentSeverity.Sev3 => new(severity, TimeSpan.FromHours(1), TimeSpan.FromHours(12), TimeSpan.FromHours(2)),
            _ => new(severity, TimeSpan.FromHours(4), TimeSpan.FromDays(2), TimeSpan.FromHours(8)),
        };
    }
}
