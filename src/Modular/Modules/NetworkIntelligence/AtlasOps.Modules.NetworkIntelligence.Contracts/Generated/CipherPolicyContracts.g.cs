namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CipherPolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record CipherPolicyRecord(Guid Id, string Name, string Owner, CipherPolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CipherPolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CipherPolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CipherPolicyQuery(string? SearchText, CipherPolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CipherPolicyPage(IReadOnlyList<CipherPolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CipherPolicyMutation(bool Succeeded, string Code, string Message, CipherPolicyRecord? Record, CipherPolicyEvent? Event);
public interface ICipherPolicyRepository
{
    ValueTask<CipherPolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CipherPolicyPage> QueryAsync(CipherPolicyQuery query, CancellationToken cancellationToken);
    ValueTask<CipherPolicyMutation> SaveAsync(CipherPolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICipherPolicyEventSink { ValueTask PublishAsync(CipherPolicyEvent domainEvent, CancellationToken cancellationToken); }