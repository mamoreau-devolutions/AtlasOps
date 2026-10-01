namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SessionRoutingState { Draft, Active, Paused, Completed, Archived }
public sealed record SessionRoutingRecord(Guid Id, string Name, string Owner, SessionRoutingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SessionRoutingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SessionRoutingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SessionRoutingQuery(string? SearchText, SessionRoutingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SessionRoutingPage(IReadOnlyList<SessionRoutingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SessionRoutingMutation(bool Succeeded, string Code, string Message, SessionRoutingRecord? Record, SessionRoutingEvent? Event);
public interface ISessionRoutingRepository
{
    ValueTask<SessionRoutingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SessionRoutingPage> QueryAsync(SessionRoutingQuery query, CancellationToken cancellationToken);
    ValueTask<SessionRoutingMutation> SaveAsync(SessionRoutingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISessionRoutingEventSink { ValueTask PublishAsync(SessionRoutingEvent domainEvent, CancellationToken cancellationToken); }