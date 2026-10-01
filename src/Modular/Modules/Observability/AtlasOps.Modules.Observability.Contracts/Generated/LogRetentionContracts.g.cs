namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LogRetentionState { Draft, Active, Paused, Completed, Archived }
public sealed record LogRetentionRecord(Guid Id, string Name, string Owner, LogRetentionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LogRetentionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LogRetentionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LogRetentionQuery(string? SearchText, LogRetentionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LogRetentionPage(IReadOnlyList<LogRetentionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LogRetentionMutation(bool Succeeded, string Code, string Message, LogRetentionRecord? Record, LogRetentionEvent? Event);
public interface ILogRetentionRepository
{
    ValueTask<LogRetentionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LogRetentionPage> QueryAsync(LogRetentionQuery query, CancellationToken cancellationToken);
    ValueTask<LogRetentionMutation> SaveAsync(LogRetentionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILogRetentionEventSink { ValueTask PublishAsync(LogRetentionEvent domainEvent, CancellationToken cancellationToken); }