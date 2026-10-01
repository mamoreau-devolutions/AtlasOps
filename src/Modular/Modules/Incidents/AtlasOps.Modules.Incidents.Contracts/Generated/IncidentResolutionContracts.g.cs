namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IncidentResolutionState { Draft, Active, Paused, Completed, Archived }
public sealed record IncidentResolutionRecord(Guid Id, string Name, string Owner, IncidentResolutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IncidentResolutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IncidentResolutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IncidentResolutionQuery(string? SearchText, IncidentResolutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IncidentResolutionPage(IReadOnlyList<IncidentResolutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IncidentResolutionMutation(bool Succeeded, string Code, string Message, IncidentResolutionRecord? Record, IncidentResolutionEvent? Event);
public interface IIncidentResolutionRepository
{
    ValueTask<IncidentResolutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IncidentResolutionPage> QueryAsync(IncidentResolutionQuery query, CancellationToken cancellationToken);
    ValueTask<IncidentResolutionMutation> SaveAsync(IncidentResolutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIncidentResolutionEventSink { ValueTask PublishAsync(IncidentResolutionEvent domainEvent, CancellationToken cancellationToken); }