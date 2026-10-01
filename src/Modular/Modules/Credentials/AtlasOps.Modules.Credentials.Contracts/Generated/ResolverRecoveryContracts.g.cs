namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ResolverRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record ResolverRecoveryRecord(Guid Id, string Name, string Owner, ResolverRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ResolverRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ResolverRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ResolverRecoveryQuery(string? SearchText, ResolverRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ResolverRecoveryPage(IReadOnlyList<ResolverRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ResolverRecoveryMutation(bool Succeeded, string Code, string Message, ResolverRecoveryRecord? Record, ResolverRecoveryEvent? Event);
public interface IResolverRecoveryRepository
{
    ValueTask<ResolverRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ResolverRecoveryPage> QueryAsync(ResolverRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<ResolverRecoveryMutation> SaveAsync(ResolverRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IResolverRecoveryEventSink { ValueTask PublishAsync(ResolverRecoveryEvent domainEvent, CancellationToken cancellationToken); }