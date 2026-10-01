namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SessionHealthState { Draft, Active, Paused, Completed, Archived }
public sealed record SessionHealthRecord(Guid Id, string Name, string Owner, SessionHealthState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SessionHealthCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SessionHealthEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SessionHealthQuery(string? SearchText, SessionHealthState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SessionHealthPage(IReadOnlyList<SessionHealthRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SessionHealthMutation(bool Succeeded, string Code, string Message, SessionHealthRecord? Record, SessionHealthEvent? Event);
public interface ISessionHealthRepository
{
    ValueTask<SessionHealthRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SessionHealthPage> QueryAsync(SessionHealthQuery query, CancellationToken cancellationToken);
    ValueTask<SessionHealthMutation> SaveAsync(SessionHealthRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISessionHealthEventSink { ValueTask PublishAsync(SessionHealthEvent domainEvent, CancellationToken cancellationToken); }