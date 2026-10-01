namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ResolverAccessState { Draft, Active, Paused, Completed, Archived }
public sealed record ResolverAccessRecord(Guid Id, string Name, string Owner, ResolverAccessState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ResolverAccessCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ResolverAccessEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ResolverAccessQuery(string? SearchText, ResolverAccessState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ResolverAccessPage(IReadOnlyList<ResolverAccessRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ResolverAccessMutation(bool Succeeded, string Code, string Message, ResolverAccessRecord? Record, ResolverAccessEvent? Event);
public interface IResolverAccessRepository
{
    ValueTask<ResolverAccessRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ResolverAccessPage> QueryAsync(ResolverAccessQuery query, CancellationToken cancellationToken);
    ValueTask<ResolverAccessMutation> SaveAsync(ResolverAccessRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IResolverAccessEventSink { ValueTask PublishAsync(ResolverAccessEvent domainEvent, CancellationToken cancellationToken); }