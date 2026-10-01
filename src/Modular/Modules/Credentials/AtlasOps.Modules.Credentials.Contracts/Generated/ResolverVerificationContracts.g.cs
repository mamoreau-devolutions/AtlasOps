namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ResolverVerificationState { Draft, Active, Paused, Completed, Archived }
public sealed record ResolverVerificationRecord(Guid Id, string Name, string Owner, ResolverVerificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ResolverVerificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ResolverVerificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ResolverVerificationQuery(string? SearchText, ResolverVerificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ResolverVerificationPage(IReadOnlyList<ResolverVerificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ResolverVerificationMutation(bool Succeeded, string Code, string Message, ResolverVerificationRecord? Record, ResolverVerificationEvent? Event);
public interface IResolverVerificationRepository
{
    ValueTask<ResolverVerificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ResolverVerificationPage> QueryAsync(ResolverVerificationQuery query, CancellationToken cancellationToken);
    ValueTask<ResolverVerificationMutation> SaveAsync(ResolverVerificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IResolverVerificationEventSink { ValueTask PublishAsync(ResolverVerificationEvent domainEvent, CancellationToken cancellationToken); }