namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CipherValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record CipherValidationRecord(Guid Id, string Name, string Owner, CipherValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CipherValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CipherValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CipherValidationQuery(string? SearchText, CipherValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CipherValidationPage(IReadOnlyList<CipherValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CipherValidationMutation(bool Succeeded, string Code, string Message, CipherValidationRecord? Record, CipherValidationEvent? Event);
public interface ICipherValidationRepository
{
    ValueTask<CipherValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CipherValidationPage> QueryAsync(CipherValidationQuery query, CancellationToken cancellationToken);
    ValueTask<CipherValidationMutation> SaveAsync(CipherValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICipherValidationEventSink { ValueTask PublishAsync(CipherValidationEvent domainEvent, CancellationToken cancellationToken); }