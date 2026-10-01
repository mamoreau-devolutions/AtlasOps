namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CipherClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record CipherClassificationRecord(Guid Id, string Name, string Owner, CipherClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CipherClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CipherClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CipherClassificationQuery(string? SearchText, CipherClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CipherClassificationPage(IReadOnlyList<CipherClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CipherClassificationMutation(bool Succeeded, string Code, string Message, CipherClassificationRecord? Record, CipherClassificationEvent? Event);
public interface ICipherClassificationRepository
{
    ValueTask<CipherClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CipherClassificationPage> QueryAsync(CipherClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<CipherClassificationMutation> SaveAsync(CipherClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICipherClassificationEventSink { ValueTask PublishAsync(CipherClassificationEvent domainEvent, CancellationToken cancellationToken); }