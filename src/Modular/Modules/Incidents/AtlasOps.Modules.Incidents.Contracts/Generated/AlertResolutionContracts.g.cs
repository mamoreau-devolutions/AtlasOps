namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AlertResolutionState { Draft, Active, Paused, Completed, Archived }
public sealed record AlertResolutionRecord(Guid Id, string Name, string Owner, AlertResolutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AlertResolutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AlertResolutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AlertResolutionQuery(string? SearchText, AlertResolutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AlertResolutionPage(IReadOnlyList<AlertResolutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AlertResolutionMutation(bool Succeeded, string Code, string Message, AlertResolutionRecord? Record, AlertResolutionEvent? Event);
public interface IAlertResolutionRepository
{
    ValueTask<AlertResolutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AlertResolutionPage> QueryAsync(AlertResolutionQuery query, CancellationToken cancellationToken);
    ValueTask<AlertResolutionMutation> SaveAsync(AlertResolutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAlertResolutionEventSink { ValueTask PublishAsync(AlertResolutionEvent domainEvent, CancellationToken cancellationToken); }