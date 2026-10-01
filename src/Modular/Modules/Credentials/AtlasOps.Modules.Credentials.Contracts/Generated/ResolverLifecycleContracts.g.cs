namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ResolverLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record ResolverLifecycleRecord(Guid Id, string Name, string Owner, ResolverLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ResolverLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ResolverLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ResolverLifecycleQuery(string? SearchText, ResolverLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ResolverLifecyclePage(IReadOnlyList<ResolverLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ResolverLifecycleMutation(bool Succeeded, string Code, string Message, ResolverLifecycleRecord? Record, ResolverLifecycleEvent? Event);
public interface IResolverLifecycleRepository
{
    ValueTask<ResolverLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ResolverLifecyclePage> QueryAsync(ResolverLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<ResolverLifecycleMutation> SaveAsync(ResolverLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IResolverLifecycleEventSink { ValueTask PublishAsync(ResolverLifecycleEvent domainEvent, CancellationToken cancellationToken); }