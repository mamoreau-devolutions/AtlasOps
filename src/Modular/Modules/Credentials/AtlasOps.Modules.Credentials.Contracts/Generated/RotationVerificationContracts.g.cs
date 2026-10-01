namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RotationVerificationState { Draft, Active, Paused, Completed, Archived }
public sealed record RotationVerificationRecord(Guid Id, string Name, string Owner, RotationVerificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RotationVerificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RotationVerificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RotationVerificationQuery(string? SearchText, RotationVerificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RotationVerificationPage(IReadOnlyList<RotationVerificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RotationVerificationMutation(bool Succeeded, string Code, string Message, RotationVerificationRecord? Record, RotationVerificationEvent? Event);
public interface IRotationVerificationRepository
{
    ValueTask<RotationVerificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RotationVerificationPage> QueryAsync(RotationVerificationQuery query, CancellationToken cancellationToken);
    ValueTask<RotationVerificationMutation> SaveAsync(RotationVerificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRotationVerificationEventSink { ValueTask PublishAsync(RotationVerificationEvent domainEvent, CancellationToken cancellationToken); }