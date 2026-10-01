namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SessionPolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record SessionPolicyRecord(Guid Id, string Name, string Owner, SessionPolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SessionPolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SessionPolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SessionPolicyQuery(string? SearchText, SessionPolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SessionPolicyPage(IReadOnlyList<SessionPolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SessionPolicyMutation(bool Succeeded, string Code, string Message, SessionPolicyRecord? Record, SessionPolicyEvent? Event);
public interface ISessionPolicyRepository
{
    ValueTask<SessionPolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SessionPolicyPage> QueryAsync(SessionPolicyQuery query, CancellationToken cancellationToken);
    ValueTask<SessionPolicyMutation> SaveAsync(SessionPolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISessionPolicyEventSink { ValueTask PublishAsync(SessionPolicyEvent domainEvent, CancellationToken cancellationToken); }