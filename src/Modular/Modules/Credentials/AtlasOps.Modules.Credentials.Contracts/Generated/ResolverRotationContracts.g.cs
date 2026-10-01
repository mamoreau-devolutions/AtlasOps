namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ResolverRotationState { Draft, Active, Paused, Completed, Archived }
public sealed record ResolverRotationRecord(Guid Id, string Name, string Owner, ResolverRotationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ResolverRotationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ResolverRotationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ResolverRotationQuery(string? SearchText, ResolverRotationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ResolverRotationPage(IReadOnlyList<ResolverRotationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ResolverRotationMutation(bool Succeeded, string Code, string Message, ResolverRotationRecord? Record, ResolverRotationEvent? Event);
public interface IResolverRotationRepository
{
    ValueTask<ResolverRotationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ResolverRotationPage> QueryAsync(ResolverRotationQuery query, CancellationToken cancellationToken);
    ValueTask<ResolverRotationMutation> SaveAsync(ResolverRotationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IResolverRotationEventSink { ValueTask PublishAsync(ResolverRotationEvent domainEvent, CancellationToken cancellationToken); }